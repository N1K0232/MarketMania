using MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface ISKUCodeGenerator
{
    Task GenerateAsync(Product product, CancellationToken cancellationToken = default);
}