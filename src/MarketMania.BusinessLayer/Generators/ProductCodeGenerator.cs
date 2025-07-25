using MarketMania.BusinessLayer.Generators.Interfaces;

namespace MarketMania.BusinessLayer.Generators;

public class ProductCodeGenerator : IProductCodeGenerator
{
    public Task<string> GenerateBarcodeAsync(CancellationToken cancellationToken = default)
    {
        var random = new Random();
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
        return Task.FromResult(code + checksum.ToString());
    }

    public Task<string> GenerateCodeAsync(CancellationToken cancellationToken = default)
    {
        var code = $"PRD-{Guid.CreateVersion7().ToString("N")[..8].ToUpper()}";
        return Task.FromResult(code);
    }

    public Task<string> GenerateSerialNumberAsync(CancellationToken cancellationToken = default)
    {
        var serialNumber = $"SN-{Guid.CreateVersion7().ToString("N")[..12].ToUpper()}";
        return Task.FromResult(serialNumber);
    }

    public Task<string> GenerateSKUAsync(CancellationToken cancellationToken = default)
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Random.Shared.Next(1000, 9999);

        return Task.FromResult($"SKU-{date}-{random}");
    }
}