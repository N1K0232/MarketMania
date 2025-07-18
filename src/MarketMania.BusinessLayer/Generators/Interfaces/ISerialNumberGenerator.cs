namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface ISerialNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}