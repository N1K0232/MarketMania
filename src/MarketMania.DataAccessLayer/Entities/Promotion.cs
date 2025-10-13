using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Promotion : BaseEntity
{
    public string Name { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Product>? Products { get; set; }
}