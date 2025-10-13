using System.Net.Http.Json;
using MarketMania.Clients.Interfaces;
using MarketMania.Clients.Models.Sentiment;

namespace MarketMania.Clients;

public class SentimentAnalysisClient(HttpClient httpClient) : ISentimentAnalysisClient
{
    public async Task<SentimentResponse?> GetPredictionAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<SentimentResponse>($"analyze/?text={text}", cancellationToken).ConfigureAwait(false);
        return response;
    }
}