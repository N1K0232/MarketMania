using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Product : BaseEntity
{
    public Guid BrandId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid SupplierId { get; set; }

    public Guid? PromotionId { get; set; }

    public string Name { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Subtitle { get; set; }

    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public double? DiscountPercentage { get; set; }

    public double? Taxes { get; set; }

    public decimal? ShippingCost { get; set; }

    public decimal TotalPrice { get; set; }

    public bool NotifyAvailability { get; set; }

    public int RatingsCount { get; set; }

    public double? RatingsAverage { get; set; }

    public string? ImageUrl { get; set; }

    public int ImagesCount { get; set; }

    public IEnumerable<string> Tags { get; set; } = null!;

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    public bool IsFeatured { get; set; }

    public string Barcode { get; set; } = null!;

    public int? SKU { get; set; }

    public string SKUCode { get; set; } = null!;

    public string Code { get; set; } = null!;

    public double Weight { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double Length { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string WarehouseLocation { get; set; } = null!;

    public int MinimumStockAlert { get; set; }

    public bool IsBackorderable { get; set; }

    public DateOnly? RestockDate { get; set; }

    public TimeOnly? RestockTime { get; set; }

    public bool NotifyRestockDate { get; set; }

    public int ViewCount { get; set; }

    public int PurchaseCount { get; set; }

    public DateTime? LastPurchasedAt { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Promotion? Promotion { get; set; }

    public virtual ICollection<Image> Images { get; set; } = [];

    public virtual ICollection<Rating> Ratings { get; set; } = [];

    public virtual ICollection<Specification> Specifications { get; set; } = [];

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = [];
}