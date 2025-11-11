using MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface ICodeGenerator
{
    Task GenerateAsync(Product product, CancellationToken cancellationToken = default);
}