using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.BusinessLayer.Generators;

public class SerialNumberGenerator(IApplicationDbContext applicationDbContext) : ISerialNumberGenerator
{
    public async Task GenerateAsync(Product product, CancellationToken cancellationToken = default)
    {
        string serialNumber;
        var query = applicationDbContext.GetData<Product>();

        do
        {
            serialNumber = $"SN-{Guid.CreateVersion7().ToString("N")[..12].ToUpper()}";
        } while (await query.AnyAsync(p => p.SerialNumber == serialNumber, cancellationToken).ConfigureAwait(false));

        product.SerialNumber = serialNumber;
    }
}