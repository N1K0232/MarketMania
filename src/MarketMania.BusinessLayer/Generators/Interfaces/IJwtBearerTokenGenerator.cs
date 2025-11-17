using System.Security.Claims;
using MarketMania.Authentication.Entities;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface IJwtBearerTokenGenerator
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    Task<string> GenerateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}