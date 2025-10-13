using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public virtual ICollection<Product>? Products { get; set; }
}