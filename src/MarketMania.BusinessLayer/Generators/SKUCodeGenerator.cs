using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.BusinessLayer.Generators;

public class SKUCodeGenerator(IApplicationDbContext applicationDbContext) : ISKUCodeGenerator
{
    public async Task GenerateAsync(Product product, CancellationToken cancellationToken = default)
    {
        string? skuCode;
        var query = applicationDbContext.GetData<Product>();

        var date = DateTime.UtcNow.ToString("yyyyMMdd");

        do
        {
            var random = Random.Shared.Next(1000, 9999);
            skuCode = $"SKU-{date}-{random}";
        } while (await query.AnyAsync(p => p.SKUCode == skuCode, cancellationToken).ConfigureAwait(false));

        product.SKUCode = skuCode;
    }
}