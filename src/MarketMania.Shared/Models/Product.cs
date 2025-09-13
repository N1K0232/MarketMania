namespace MarketMania.Shared.Models;

public class Product
{
    public Guid Id { get; set; }

    public string Brand { get; set; }

    public string Category { get; set; }

    public string Supplier { get; set; }

    public string Name { get; set; }

    public string Subtitle { get; set; }

    public string Description { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public double? DiscountPercentage { get; set; }

    public double? Taxes { get; set; }

    public decimal? ShippingCost { get; set; }

    public decimal TotalPrice { get; set; }

    public int RatingsCount { get; set; }

    public double? RatingsAverage { get; set; }

    public string ImageUrl { get; set; }

    public string Tags { get; set; }

    public string SeoTitle { get; set; }

    public string SeoDescription { get; set; }

    public bool IsFeatured { get; set; }

    public string Barcode { get; set; }

    public int? SKU { get; set; }

    public double Weight { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double Length { get; set; }

    public string SerialNumber { get; set; }

    public string WarehouseLocation { get; set; }

    public int MinStockAlert { get; set; }

    public bool IsBackorderable { get; set; }

    public DateTime? RestockDate { get; set; }

    public int ViewCount { get; set; }

    public int PurchaseCount { get; set; }

    public DateTime? LastPurchasedAt { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public IEnumerable<Image> Images { get; set; }

    public IEnumerable<Rating> Ratings { get; set; }

    public IEnumerable<Specification> Specifications { get; set; }
}