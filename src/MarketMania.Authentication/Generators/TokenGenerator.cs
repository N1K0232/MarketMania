using System.Security.Claims;
using MarketMania.Authentication.Entities;
using MarketMania.Authentication.Generators.Interfaces;
using Microsoft.AspNetCore.Identity;
using SimpleAuthentication.JwtBearer;

namespace MarketMania.Authentication.Generators;

public class TokenGenerator(UserManager<ApplicationUser> userManager, IJwtBearerService jwtBearerService) : ITokenGenerator
{
    public async Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        await userManager.UpdateSecurityStampAsync(user);
        var userRoles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new Claim(ClaimTypes.SerialNumber, user.SecurityStamp)
        }.Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = await jwtBearerService.CreateTokenAsync(user.UserName, [.. claims]);
        return token;
    }
}