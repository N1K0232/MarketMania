
namespace MarketMania.Authentication.Generators.Interfaces;

public interface IQRCodeGenerator
{
    Task<Stream> GenerateAsync(string email, string? secret, CancellationToken cancellationToken = default);
}