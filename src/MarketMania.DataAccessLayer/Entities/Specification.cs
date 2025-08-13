using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Specification : BaseEntity
{
    public Guid ProductId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual Product Product { get; set; }
}