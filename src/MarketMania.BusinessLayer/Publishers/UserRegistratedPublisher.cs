using System.Text;
using MarketMania.Authentication;
using MarketMania.Authentication.DataProtection.Interfaces;
using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Resources;
using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Models.Email;
using MarketMania.Contracts;
using MarketMania.Shared.Models.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SimpleTransit;

namespace MarketMania.BusinessLayer.Publishers;

public class UserRegistratedPublisher(UserManager<ApplicationUser> userManager, ITimeLimitedDataProtectionService dataProtectionService, IEmailClient emailClient, IPageService pageService) : INotificationHandler<UserRegistrated>, INotificationHandler<UserVerified>
{
    public async Task HandleAsync(UserRegistrated message, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(message.Email).ConfigureAwait(false);
        var secret = await dataProtectionService.ProtectAsync(user!.Id.ToString(), TimeSpan.FromMinutes(15), cancellationToken).ConfigureAwait(false);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

        var encodedSecret = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(secret));
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var endpoint = await pageService.GetEndpointAsync("verifyemail", new { secret = encodedSecret, token = encodedToken }, cancellationToken).ConfigureAwait(false);

        var emailMessage = new EmailMessage
        {
            SenderEmail = "no-reply@marketmania.com",
            SenderName = "Market Mania",
            To = [message.Email],
            Subject = "Verify your email",
            TextContent = string.Format(Messages.VerifyEmail, endpoint)
        };

        await emailClient.SendAsync(emailMessage, cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(UserVerified message, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(message.Email).ConfigureAwait(false);
        await userManager.AddToRoleAsync(user!, RoleNames.User).ConfigureAwait(false);

        var emailMessage = new EmailMessage
        {
            SenderEmail = "no-reply@marketmania.com",
            SenderName = "Market Mania",
            To = [message.Email],
            Subject = "Email successfully verified",
            TextContent = "You have successfully verified"
        };

        await emailClient.SendAsync(emailMessage, cancellationToken).ConfigureAwait(false);
    }
}