using MarketMania.DataAccessLayer.Entities;

namespace MarketMania.DataAccessLayer.Stores.Interfaces;

public interface IProductStore
{
    Task GenerateBarcodeAsync(Product product, CancellationToken cancellationToken = default);

    Task GenerateCodeAsync(Product product, CancellationToken cancellationToken = default);

    Task GenerateSerialNumberAsync(Product product, CancellationToken cancellationToken = default);

    Task GenerateSKUCodeAsync(Product product, CancellationToken cancellationToken = default);
}