using MarketMania.Shared.Models;

namespace MarketMania.BusinessLayer.Generators.Interfaces;

public interface IPdfGenerator
{
    Task<Stream> CreateAsync(string content, PdfOptions options = null, CancellationToken cancellationToken = default);
}