using MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface ISerialNumberGenerator
{
    Task GenerateAsync(Product product, CancellationToken cancellationToken = default);
}