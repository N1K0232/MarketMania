using System.Net.Mime;
using System.Net.Sockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using MarketMania.Authentication.DataProtection.Interfaces;
using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Notifications;
using MarketMania.Shared.Models.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OperationResults;
using SimpleAuthentication;
using SimpleTransit;
using TinyHelpers.Extensions;

namespace MarketMania.BusinessLayer.Services;

public class IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtBearerTokenGenerator jwtBearerTokenGenerator, INotificationPublisher notificationPublisher, IQRCodeGenerator qrCodeGenerator, IDataProtectionService dataProtectionService, IMapper mapper) : IIdentityService
{
    public async Task<Result<StreamFileContent>> GetQRCodeAsync(string token, CancellationToken cancellationToken)
    {
        ApplicationUser? user;

        try
        {
            var userId = await dataProtectionService.UnprotectAsync(token, cancellationToken);
            user = await userManager.FindByIdAsync(userId);
        }
        catch (CryptographicException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to get the qr code", ex.Message);
        }

        if (user is null || (await userManager.GetAuthenticatorKeyAsync(user)).HasValue())
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        var qrCodeStream = await qrCodeGenerator.GenerateAsync(user, cancellationToken);
        return new StreamFileContent(qrCodeStream, MediaTypeNames.Image.Png);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Result.Fail(FailureReasons.ClientError, "Couldn't sign in", "Invalid email or password");
        }

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

        var accessToken = await jwtBearerTokenGenerator.GenerateAccessTokenAsync(user, cancellationToken);
        var refreshToken = await jwtBearerTokenGenerator.GenerateRefreshTokenAsync(user, cancellationToken);

        return new AuthResponse(accessToken, refreshToken);
    }

    public async Task<Result> LogoutAsync(CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
        await signInManager.Context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Result.Ok();
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var user = await jwtBearerTokenGenerator.ValidateAccessTokenAsync(request.AccessToken, cancellationToken);
        if (user is null)
        {
            return Result.Fail(FailureReasons.ClientError, "Invalid access token");
        }

        var dbUser = await userManager.FindByIdAsync(user.GetClaimValue(ClaimTypes.NameIdentifier)!);
        if (dbUser?.RefreshToken is null || dbUser?.RefreshTokenExpirationDate < DateTimeOffset.UtcNow || dbUser?.RefreshToken != request.RefreshToken)
        {
            return Result.Fail(FailureReasons.ClientError, "Invalid refresh token");
        }

        if(dbUser.TwoFactorEnabled)
        {
            var twoFactorToken = await dataProtectionService.ProtectAsync(dbUser.Id.ToString(), TimeSpan.FromMinutes(15), cancellationToken);
            return new AuthResponse(twoFactorToken);
        }

        var accessToken = await jwtBearerTokenGenerator.GenerateAccessTokenAsync(dbUser, cancellationToken);
        var refreshToken = await jwtBearerTokenGenerator.GenerateRefreshTokenAsync(dbUser, cancellationToken);

        return new AuthResponse(accessToken, refreshToken);
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

        try
        {
            await notificationPublisher.NotifyAsync(new UserRegistrated(request.Email), cancellationToken);
            return Result.Ok();
        }
        catch (SocketException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Email not sent", ex.Message);
        }
    }

    public async Task<Result<AuthResponse>> ValidateTwoFactorAsync(TwoFactorValidationRequest request, CancellationToken cancellationToken)
    {
        ApplicationUser? user;

        try
        {
            var userId = await dataProtectionService.UnprotectAsync(request.Token, cancellationToken);
            user = await userManager.FindByIdAsync(userId);
        }
        catch (CryptographicException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to get the qr code", ex.Message);
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

        var accessToken = await jwtBearerTokenGenerator.GenerateAccessTokenAsync(user, cancellationToken);
        var refreshToken = await jwtBearerTokenGenerator.GenerateRefreshTokenAsync(user, cancellationToken);

        return new AuthResponse(accessToken, refreshToken);
    }

    public async Task<Result> VerifyEmailAsync(string secret, string token, CancellationToken cancellationToken)
    {
        ApplicationUser? user;

        var decodedSecret = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(secret));
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        try
        {
            var userId = await dataProtectionService.UnprotectAsync(decodedSecret, cancellationToken);
            user = await userManager.FindByIdAsync(userId);
        }
        catch (CryptographicException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to verify your email", ex.Message);
        }

        if (user is null)
        {
            return Result.Fail(FailureReasons.ClientError);
        }

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to verify your email", "Email not verified");
        }

        try
        {
            await notificationPublisher.NotifyAsync(new UserVerified(user.Email!), cancellationToken);
            return Result.Ok();
        }
        catch (SocketException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Email not sent", ex.Message);
        }
    }
}