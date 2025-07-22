using MarketMania.Authentication.Entities;
using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Rating : BaseEntity
{
    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; }

    public string Text { get; set; }

    public int Score { get; set; }

    public double SentimentScore { get; set; }

    public bool IsPublished { get; set; }

    public virtual Product Product { get; set; }

    public virtual ApplicationUser User { get; set; }
}