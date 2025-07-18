namespace MarketMania.Shared.Models;

public class Image
{
    public Guid Id { get; set; }

    public string Path { get; set; }

    public string ContentType { get; set; }

    public long Length { get; set; }

    public bool IsPublished { get; set; }
}