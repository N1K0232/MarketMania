
namespace MarketMania.Authentication.Generators;

public interface IQRCodeGenerator
{
    Task<Stream> GenerateAsync(string email, string secret, CancellationToken cancellationToken = default);
}