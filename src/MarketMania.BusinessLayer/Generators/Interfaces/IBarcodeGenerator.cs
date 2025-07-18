namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface IBarcodeGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken = default);
}