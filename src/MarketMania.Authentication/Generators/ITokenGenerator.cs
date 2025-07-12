using MarketMania.Authentication.Entities;

namespace MarketMania.Authentication.Generators;

public interface ITokenGenerator
{
    Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}