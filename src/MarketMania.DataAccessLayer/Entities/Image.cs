using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class Image : BaseEntity
{
    public Guid ProductId { get; set; }

    public string Path { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long Length { get; set; }

    public bool IsPublished { get; set; }

    public virtual Product Product { get; set; } = null!;
}