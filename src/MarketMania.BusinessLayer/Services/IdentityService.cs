using System.Net.Mime;
using AutoMapper;
using MarketMania.Authentication;
using MarketMania.Authentication.DataProtection;
using MarketMania.Authentication.Entities;
using MarketMania.Authentication.Generators.Interfaces;
using MarketMania.BusinessLayer.Resources;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Models.Email;
using MarketMania.Contracts;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using OperationResults;
using TinyHelpers.Extensions;

namespace MarketMania.BusinessLayer.Services;

public class IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenGenerator tokenGenerator, IQRCodeGenerator qrCodeGenerator, IDataProtectionService dataProtectionService, IPageService pageService, IEmailClient emailClient, IMapper mapper) : IIdentityService
{
    public async Task<Result<StreamFileContent>> GetQRCodeAsync(string token, CancellationToken cancellationToken)
    {
        ApplicationUser user;

        try
        {
            var userId = await dataProtectionService.UnprotectAsync(token, cancellationToken);
            user = await userManager.FindByIdAsync(userId);
        }
        catch
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        if (user is null || (await userManager.GetAuthenticatorKeyAsync(user)).HasValue())
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        await userManager.ResetAuthenticatorKeyAsync(user);
        var secret = await userManager.GetAuthenticatorKeyAsync(user);

        var stream = await qrCodeGenerator.GenerateAsync(user.Email, secret, cancellationToken);
        return new StreamFileContent(stream, MediaTypeNames.Image.Png);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, false, false);

        if (!signInResult.Succeeded)
        {
            if (signInResult.RequiresTwoFactor)
            {
                var twoFactorToken = await dataProtectionService.ProtectAsync(user.Id.ToString(), TimeSpan.FromMinutes(15), cancellationToken);
                return new AuthResponse(twoFactorToken);
            }

            if (signInResult.IsLockedOut)
            {
                return Result.Fail(FailureReasons.Unauthorized, "User locked out", $"You're locked out until {user.LockoutEnd}");
            }

            await userManager.AccessFailedAsync(user);
            return Result.Fail(FailureReasons.ClientError, "Couldn't sign in", "Invalid email or password");
        }

        var accessToken = await tokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthResponse(accessToken);
    }

    public async Task<Result> LogoutAsync(CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
        await signInManager.Context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Result.Ok();
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<ApplicationUser>(request);
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var detail = string.Join(',', result.Errors.Select(e => e.Description));
            return Result.Fail(FailureReasons.ClientError, "Couldn't register the user", detail);
        }

        var token = await dataProtectionService.ProtectAsync(user.Id.ToString(), TimeSpan.FromMinutes(15), cancellationToken);
        var secret = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var page = await pageService.GetPageAsync("/Accounts/VerifyEmail", new { token, secret }, cancellationToken);
        if (!page.HasValue())
        {
            return Result.Fail(FailureReasons.ClientError, "Page not found", "Page not found");
        }

        var emailMessage = new EmailMessage
        {
            To = [request.Email],
            TextContent = string.Format(Messages.VerifyEmail, page)
        };

        var response = await emailClient.SendAsync(emailMessage, cancellationToken);
        if (!response.Succeed)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to complete registration");
        }

        return Result.Ok();
    }

    public async Task<Result<AuthResponse>> ValidateTwoFactorAsync(TwoFactorValidationRequest request, CancellationToken cancellationToken)
    {
        ApplicationUser user;

        try
        {
            var userId = await dataProtectionService.UnprotectAsync(request.Token, cancellationToken);
            user = await userManager.FindByIdAsync(userId);
        }
        catch
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        if (user is null)
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        var isValidTotpCode = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);
        if (!isValidTotpCode)
        {
            return Result.Fail(FailureReasons.ClientError, "Invalid two factor code");
        }

        var accessToken = await tokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthResponse(accessToken);
    }

    public async Task<Result> VerifyEmailAsync(string token, string secret, CancellationToken cancellationToken)
    {
        ApplicationUser user;

        try
        {
            var userId = await dataProtectionService.UnprotectAsync(token, cancellationToken);
            user = await userManager.FindByIdAsync(userId);
        }
        catch
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        if (user is null)
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        var result = await userManager.ConfirmEmailAsync(user, secret);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, RoleNames.User);
            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ClientError, "Unable to verify your email", "Email not verified");
    }
}