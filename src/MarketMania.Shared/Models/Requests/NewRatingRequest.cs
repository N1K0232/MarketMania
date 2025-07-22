namespace MarketMania.Shared.Models.Requests;

public record class NewRatingRequest(string Title, string Text, int Score);