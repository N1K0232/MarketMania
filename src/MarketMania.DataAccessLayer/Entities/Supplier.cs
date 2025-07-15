using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; }

    public string City { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}