using System.Net.Http.Json;
using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Models.Pdf;

namespace MarketMania.Clients;

public class PdfSmithClient(HttpClient httpClient) : IPdfSmithClient
{
    public async Task<Stream> GeneratePdfAsync(PdfGenerationRequest request, CancellationToken cancellationToken = default)
    {
        using var httpResponse = await httpClient.PostAsJsonAsync("api/pdf", request, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        return stream;
    }
}