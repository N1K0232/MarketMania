namespace MarketMania.Shared.Models;

public record class Promotion(Guid Id, string Name, decimal DiscountPercentage, DateTimeOffset StartDate, DateTimeOffset EndDate, bool IsActive);