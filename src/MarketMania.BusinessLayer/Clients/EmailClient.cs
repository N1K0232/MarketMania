using MailKit.Net.Smtp;
using MarketMania.BusinessLayer.Clients.Interfaces;
using MarketMania.BusinessLayer.Settings;
using MarketMania.Shared.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using TinyHelpers.Extensions;

namespace MarketMania.BusinessLayer.Clients;

public class EmailClient(IOptions<EmailSettings> emailSettingsOptions) : IEmailClient
{
    private readonly EmailSettings emailSettings = emailSettingsOptions.Value;

    private SmtpClient client = new SmtpClient();
    private bool disposed = false;

    public async Task<SendEmailResponse> SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        var message = CreateMessage(emailMessage);

        if (emailSettings.IgnoreServerCertificateErrors)
        {
            client.ServerCertificateValidationCallback = (_, _, _, _) => true;
        }

        try
        {
            await client.ConnectAsync(emailSettings.Host, emailSettings.Port, emailSettings.UseSsl, cancellationToken);

            if (emailSettings.UserName.HasValue() && emailSettings.Password.HasValue())
            {
                await client.AuthenticateAsync(emailSettings.UserName, emailSettings.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            return new SendEmailResponse(false, ex.Message);
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken);
        }

        var response = new SendEmailResponse(true);
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