namespace MarketMania.Shared.Models.Requests;

public record class SaveProductRequest
(
    Guid BrandId,
    Guid CategoryId,
    Guid SupplierId,
    Guid? PromotionId,
    string Name,
    string Title,
    string? Subtitle,
    string Description,
    int Quantity,
    decimal Price,
    decimal? ShippingCost,
    double? DiscountPercentage,
    double? Taxes,
    bool NotifyAvailability,
    string[] Tags,
    string? SeoTitle,
    string? SeoDescription,
    int? SKU,
    double Weight,
    double Width,
    double Height,
    double Length,
    bool IsBackorderable,
    string WarehouseLocation,
    int MinimumStockAlert,
    DateOnly? RestockDate,
    TimeOnly? RestockTime,
    bool NotifyRestockDate
);