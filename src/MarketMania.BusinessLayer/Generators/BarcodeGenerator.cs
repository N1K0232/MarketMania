using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.BusinessLayer.Generators;

public class BarcodeGenerator(IApplicationDbContext applicationDbContext) : IBarcodeGenerator
{
    public async Task GenerateAsync(Product product, CancellationToken cancellationToken = default)
    {
        string? barcode;
        var random = new Random();

        var query = applicationDbContext.GetData<Product>();

        do
        {
            var code = string.Empty;

            for (var i = 0; i < 12; i++)
            {
                code += random.Next(0, 10);
            }

            var sum = 0;

            for (var i = 0; i < 12; i++)
            {
                var digit = int.Parse(code[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            var checksum = (10 - (sum % 10)) % 10;
            barcode = code + checksum.ToString();

        } while (await query.AnyAsync(p => p.Barcode == barcode, cancellationToken).ConfigureAwait(false));

        product.Barcode = barcode;
    }
}