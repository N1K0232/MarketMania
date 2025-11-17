using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using MarketMania.Authentication;
using MarketMania.Authentication.DataProtection.Interfaces;
using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Generators.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleAuthentication.JwtBearer;

namespace MarketMania.BusinessLayer.Generators;

public class JwtBearerTokenGenerator(UserManager<ApplicationUser> userManager, IJwtBearerService jwtBearerService, IDataProtectionService dataProtectionService) : IJwtBearerTokenGenerator
{
    private const int RequestPerWindow = 5;
    private const int WindowMinutes = 1;

    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var claims = await GetOrCreateClaimsAsync(user, cancellationToken).ConfigureAwait(false);  
        var token = await jwtBearerService.CreateTokenAsync(user.UserName!, claims).ConfigureAwait(false);

        return token;
    }

    public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        using var generator = RandomNumberGenerator.Create();
        var randomNumber = new byte[256];

        generator.GetBytes(randomNumber);
        var refreshToken = await dataProtectionService.ProtectAsync(Convert.ToBase64String(randomNumber), cancellationToken).ConfigureAwait(false);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationDate = DateTimeOffset.UtcNow.AddHours(4);

        await userManager.UpdateAsync(user).ConfigureAwait(false);
        return refreshToken;
    }

    public async Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var validationResult = await jwtBearerService.TryValidateTokenAsync(accessToken, false).ConfigureAwait(false);
        return validationResult.IsValid ? validationResult.Principal : null;
    }

    private async Task<IList<Claim>> GetOrCreateClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userClaims = await userManager.GetClaimsAsync(user).ConfigureAwait(false);
        if (userClaims.Count > 0)
        {
            return userClaims;
        }

        await userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        var userRoles = await userManager.GetRolesAsync(user).ConfigureAwait(false);

        var hostName = Dns.GetHostName();
        var addresses = await Dns.GetHostAddressesAsync(hostName, cancellationToken).ConfigureAwait(false);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
            new Claim(ClaimTypes.SerialNumber, user.SecurityStamp!),
            new Claim(CustomClaimTypes.PermitLimit, RequestPerWindow.ToString()),
            new Claim(CustomClaimTypes.Window, WindowMinutes.ToString()),
            new Claim(ClaimTypes.Dns, hostName)
        }
        .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role)))
        .Union(addresses.Select(address => new Claim(ClaimTypes.Dns, address.ToString())));

        await userManager.AddClaimsAsync(user, claims).ConfigureAwait(false);
        return [.. claims];
    }
}