using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }
}