using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Promotion : BaseEntity
{
    public string Name { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}