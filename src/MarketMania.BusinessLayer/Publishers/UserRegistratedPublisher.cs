using System.Text;
using MarketMania.Authentication;
using MarketMania.Authentication.DataProtection;
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

public class UserRegistratedPublisher(UserManager<ApplicationUser> userManager, IDataProtectionService dataProtectionService, IEmailClient emailClient, IPageService pageService) : INotificationHandler<UserRegistrated>, INotificationHandler<UserVerified>
{
    public async Task HandleAsync(UserRegistrated message, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(message.Email);
        var secret = await dataProtectionService.ProtectAsync(user.Id.ToString(), TimeSpan.FromMinutes(15), cancellationToken);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedSecret = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(secret));
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var endpoint = await pageService.GetEndpointAsync("verifyemail", new { secret = encodedSecret, token = encodedToken }, cancellationToken);

        var emailMessage = new EmailMessage
        {
            SenderEmail = "no-reply@marketmania.com",
            SenderName = "Market Mania",
            To = [message.Email],
            Subject = "Verify your email",
            TextContent = string.Format(Messages.VerifyEmail, endpoint)
        };

        await emailClient.SendAsync(emailMessage, cancellationToken);
    }

    public async Task HandleAsync(UserVerified message, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(message.Email);
        await userManager.AddToRoleAsync(user, RoleNames.User);

        var emailMessage = new EmailMessage
        {
            SenderEmail = "no-reply@marketmania.com",
            SenderName = "Market Mania",
            To = [message.Email],
            Subject = "Email successfully verified",
            TextContent = "You have successfully verified"
        };

        await emailClient.SendAsync(emailMessage, cancellationToken);
    }
}