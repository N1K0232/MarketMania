namespace MarketMania.Clients.Models.Sentiment;

public record class SentimentResponse(string Type, float Score, float Ratio);