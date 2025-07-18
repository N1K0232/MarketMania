using MarketMania.BusinessLayer.Generators.Interfaces;

namespace MarketMania.BusinessLayer.Generators;

public class BarcodeGenerator : IBarcodeGenerator
{
    public Task<string> GenerateAsync(CancellationToken cancellationToken = default)
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
}