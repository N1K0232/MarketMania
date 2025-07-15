using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Product : BaseEntity
{
    public Guid BrandId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid SupplierId { get; set; }

    public Guid? PromotionId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public double? DiscountPercentage { get; set; }

    public double? Taxes { get; set; }

    public decimal? ShippingCost { get; set; }

    public decimal TotalPrice { get; set; }

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

    public virtual Brand Brand { get; set; }

    public virtual Category Category { get; set; }

    public virtual Supplier Supplier { get; set; }

    public virtual Promotion Promotion { get; set; }

    public virtual ICollection<Image> Images { get; set; }
}