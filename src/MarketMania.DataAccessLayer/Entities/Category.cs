using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Product>? Products { get; set; }
}