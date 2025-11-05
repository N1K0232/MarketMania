namespace MarketMania.Shared.Models.Notifications;

public record class ImageCreated(Guid ProductId, string Path, Stream Stream);