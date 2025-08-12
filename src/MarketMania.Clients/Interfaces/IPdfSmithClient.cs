using MarketMania.Clients.Models.Pdf;

namespace MarketMania.Clients.Interfaces;

public interface IPdfSmithClient
{
    Task<Stream> GeneratePdfAsync(PdfGenerationRequest request, CancellationToken cancellationToken = default);
}