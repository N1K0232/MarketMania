using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using MarketMania.Authentication;
using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Generators.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleAuthentication.JwtBearer;

namespace MarketMania.BusinessLayer.Generators;

public class JwtBearerTokenGenerator(UserManager<ApplicationUser> userManager, IJwtBearerService jwtBearerService) : IJwtBearerTokenGenerator
{
    private const int RequestPerWindow = 5;
    private const int WindowMinutes = 1;

    public async Task<string> CreateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
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

        var token = await jwtBearerService.CreateTokenAsync(user.UserName!, [.. claims]).ConfigureAwait(false);
        return token;
    }
}