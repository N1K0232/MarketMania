namespace MarketMania.Shared.Models.Requests;

public record class SavePromotionRequest(string Name, decimal DiscountPercentage, DateTimeOffset StartDate, DateTimeOffset EndDate);