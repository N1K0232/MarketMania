using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Caching.Interfaces;
using MarketMania.DataAccessLayer.Entities;
using MarketMania.Shared.Models.Notifications;
using SimpleTransit;

namespace MarketMania.BusinessLayer.Publishers;

public class ProductPublisher(IApplicationDbContext applicationDbContext, IDataContextCache cache) : INotificationHandler<ProductCreated>, INotificationHandler<ProductUpdated>, INotificationHandler<ProductDeleted>
{
    public async Task HandleAsync(ProductCreated message, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync("Products", cancellationToken).ConfigureAwait(false);

        var product = await applicationDbContext.GetAsync<Product>(message.Id, cancellationToken).ConfigureAwait(false);
        await cache.SetAsync(product!, TimeSpan.FromHours(1), cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(ProductUpdated message, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(message.Id, cancellationToken).ConfigureAwait(false);
        await cache.RemoveAsync("Products", cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(ProductDeleted message, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(message.Id, cancellationToken).ConfigureAwait(false);
        await cache.RemoveAsync("Products", cancellationToken).ConfigureAwait(false);
    }
}