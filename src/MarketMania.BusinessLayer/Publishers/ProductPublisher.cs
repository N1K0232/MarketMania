using MarketMania.Shared.Models.Notifications;
using SimpleTransit;

namespace MarketMania.BusinessLayer.Publishers;

public class ProductPublisher : INotificationHandler<ProductCreated>, INotificationHandler<ProductUpdated>, INotificationHandler<ProductDeleted>
{
    public Task HandleAsync(ProductCreated message, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task HandleAsync(ProductUpdated message, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task HandleAsync(ProductDeleted message, CancellationToken cancellationToken)
        => Task.CompletedTask;
}