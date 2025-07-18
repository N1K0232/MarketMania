namespace MarketMania.Shared.Models.Requests;

public record class SavePromotionRequest(string Name, decimal DiscountPercentage, DateTime StartDate, DateTime EndDate);