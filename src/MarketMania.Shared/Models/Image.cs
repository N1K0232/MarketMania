namespace MarketMania.Shared.Models;

public record class Image(Guid Id, string Path, string ContentType, long Length, bool IsPublished);