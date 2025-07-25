using MarketMania.BusinessLayer.Generators.Interfaces;

namespace MarketMania.BusinessLayer.Generators;

public class ChromiumPdfGenerator : IPdfGenerator
{
    public async Task<Stream> CreateAsync(string content, CancellationToken cancellationToken)
    {
        await Task.Delay(50, cancellationToken);
        return Stream.Null;
    }
}