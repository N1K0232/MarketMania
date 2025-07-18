using MarketMania.BusinessLayer.Generators.Interfaces;

namespace MarketMania.BusinessLayer.Generators;

public class SerialNumberGenerator : ISerialNumberGenerator
{
    public Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        var serialNumber = $"SN-{Guid.CreateVersion7().ToString("N")[..12].ToUpper()}";
        return Task.FromResult(serialNumber);
    }
}