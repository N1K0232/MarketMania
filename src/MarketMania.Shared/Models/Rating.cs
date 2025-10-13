namespace MarketMania.Shared.Models;

public record class Rating(Guid Id, string User, string Product, string Title, string Text, int Score, double? SentimentScore);