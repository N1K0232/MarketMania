using MarketMania.Clients.Models.Sentiment;

namespace MarketMania.Clients.Interfaces;

public interface ISentimentAnalysisClient
{
    Task<SentimentResponse?> GetPredictionAsync(string text, CancellationToken cancellationToken = default);
}