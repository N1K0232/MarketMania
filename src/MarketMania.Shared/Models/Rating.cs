namespace MarketMania.Shared.Models;

public class Rating
{
    public Guid Id { get; set; }

    public string User { get; set; }

    public string Product { get; set; }

    public string Title { get; set; }

    public string Text { get; set; }

    public int Score { get; set; }

    public double? SentimentScore { get; set; }
}