using MarketMania.Authentication.Entities;

namespace MarketMania.Authentication.Generators.Interfaces;

public interface ITokenGenerator
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}