using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Models.Pdf;
using System.Net.Http.Json;

namespace MarketMania.Clients;

public class PdfSmithClient(HttpClient httpClient) : IPdfSmithClient
{
    public async Task<Stream> GeneratePdfAsync(PdfGenerationRequest request, CancellationToken cancellationToken = default)
    {
        using var httpResponse = await httpClient.PostAsJsonAsync("api/pdf", request, cancellationToken).ConfigureAwait(false);
        httpResponse.EnsureSuccessStatusCode();

        using var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var stream = File.OpenRead(request.FileName);

        await responseStream.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        return stream;
    }
}