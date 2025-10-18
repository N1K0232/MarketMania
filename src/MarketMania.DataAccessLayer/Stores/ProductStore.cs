using MarketMania.DataAccessLayer.Entities;
using MarketMania.DataAccessLayer.Stores.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.DataAccessLayer.Stores;

public class ProductStore(IApplicationDbContext applicationDbContext) : IProductStore
{
    public async Task GenerateBarcodeAsync(Product product, CancellationToken cancellationToken = default)
    {
        string barcode;
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

    public async Task GenerateCodeAsync(Product product, CancellationToken cancellationToken = default)
    {
        string code;
        var query = applicationDbContext.GetData<Product>();

        do
        {
            code = $"PRD-{Guid.CreateVersion7().ToString("N")[..8].ToUpper()}";
        } while (await query.AnyAsync(p => p.Code == code, cancellationToken).ConfigureAwait(false));

        product.Code = code;
    }

    public async Task GenerateSerialNumberAsync(Product product, CancellationToken cancellationToken = default)
    {
        string serialNumber;
        var query = applicationDbContext.GetData<Product>();

        do
        {
            serialNumber = $"SN-{Guid.CreateVersion7().ToString("N")[..12].ToUpper()}";
        } while (await query.AnyAsync(p => p.SerialNumber == serialNumber, cancellationToken).ConfigureAwait(false));

        product.SerialNumber = serialNumber;
    }

    public async Task GenerateSKUCodeAsync(Product product, CancellationToken cancellationToken = default)
    {
        string skuCode;
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