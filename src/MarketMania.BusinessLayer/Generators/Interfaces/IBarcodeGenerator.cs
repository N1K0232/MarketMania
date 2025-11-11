using MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface IBarcodeGenerator
{
    Task GenerateAsync(Product product, CancellationToken cancellationToken = default);
}