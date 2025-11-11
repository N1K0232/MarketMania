using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.BusinessLayer.Generators;

public class CodeGenerator(IApplicationDbContext applicationDbContext) : ICodeGenerator
{
    public async Task GenerateAsync(Product product, CancellationToken cancellationToken = default)
    {
        string? code;
        var query = applicationDbContext.GetData<Product>();

        do
        {
            code = $"PRD-{Guid.CreateVersion7().ToString("N")[..8].ToUpper()}";
        } while (await query.AnyAsync(p => p.Code == code, cancellationToken).ConfigureAwait(false));

        product.Code = code;
    }
}