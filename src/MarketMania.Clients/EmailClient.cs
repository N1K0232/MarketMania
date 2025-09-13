using MailKit.Net.Smtp;
using MarketMania.Clients.Extensions;
using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Models.Email;
using MarketMania.Clients.Settings;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MarketMania.Clients;

public class EmailClient(IOptions<EmailSettings> emailSettingsOptions) : IEmailClient
{
    private readonly EmailSettings emailSettings = emailSettingsOptions.Value;

    private SmtpClient client = new();
    private bool disposed = false;

    public async Task<SendEmailResponse> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        var message = CreateMessage(emailMessage);

        if (emailSettings.IgnoreServerCertificateErrors)
        {
            client.ServerCertificateValidationCallback = (_, _, _, _) => true;
        }

        await client.ConnectAsync(emailSettings.Host, emailSettings.Port, emailSettings.UseSsl, cancellationToken).ConfigureAwait(false);

        if (emailSettings.UserName.HasValue() && emailSettings.Password.HasValue())
        {
            await client.AuthenticateAsync(emailSettings.UserName, emailSettings.Password, cancellationToken).ConfigureAwait(false);
        }

        await client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);

        var response = new SendEmailResponse(true, message.MessageId);
        return response;
    }

    private static MimeMessage CreateMessage(EmailMessage emailMessage)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(emailMessage.SenderName ?? emailMessage.SenderEmail, emailMessage.SenderEmail));
        message.To.AddRange(emailMessage.To?.Select(a => new MailboxAddress(a, a)) ?? []);
        message.Cc.AddRange(emailMessage.Cc?.Select(a => new MailboxAddress(a, a)) ?? []);
        message.Bcc.AddRange(emailMessage.Bcc?.Select(a => new MailboxAddress(a, a)) ?? []);
        message.ReplyTo.AddRange(emailMessage.ReplyTo?.Select(a => new MailboxAddress(a, a)) ?? []);

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = emailMessage.HtmlContent,
            TextBody = emailMessage.TextContent
        };

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            if (!disposed)
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, CancellationToken.None);
                }

                client.Dispose();
                client = null;

                disposed = true;
            }
        }
    }
}