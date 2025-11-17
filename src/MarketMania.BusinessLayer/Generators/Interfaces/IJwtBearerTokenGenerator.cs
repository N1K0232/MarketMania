using MarketMania.Authentication.Entities;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface IJwtBearerTokenGenerator
{
    Task<string> CreateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}