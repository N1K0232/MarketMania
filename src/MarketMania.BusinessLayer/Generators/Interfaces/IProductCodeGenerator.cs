namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface IProductCodeGenerator
{
    Task<string> GenerateBarcodeAsync(CancellationToken cancellationToken = default);

    Task<string> GenerateCodeAsync(CancellationToken cancellationToken = default);

    Task<string> GenerateSerialNumberAsync(CancellationToken cancellationToken = default);

    Task<string> GenerateSKUAsync(CancellationToken cancellationToken = default);
}