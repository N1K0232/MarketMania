using System;
using System.Collections.Generic;
using System.Text;
using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Caching.Interfaces;
using MarketMania.DataAccessLayer.Entities;
using MarketMania.Shared.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using SimpleTransit;

namespace MarketMania.BusinessLayer.Publishers;

public class BrandPublisher(IApplicationDbContext applicationDbContext, IDataContextCache cache) : INotificationHandler<BrandCreated>,
    INotificationHandler<BrandUpdated>,
    INotificationHandler<BrandDeleted>
{
    public async Task HandleAsync(BrandDeleted message, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync("brands", cancellationToken).ConfigureAwait(false);
        await cache.RemoveAsync(message.BrandId, cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(BrandUpdated message, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync("brands", cancellationToken).ConfigureAwait(false);
        await cache.RemoveAsync(message.BrandId, cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(BrandCreated message, CancellationToken cancellationToken)
    {
        var brand = await applicationDbContext.GetData<Brand>().FirstAsync(b => b.Id == message.BrandId, cancellationToken).ConfigureAwait(false);
        await cache.SetAsync(brand, cancellationToken).ConfigureAwait(false);
        await cache.RemoveAsync("brands", cancellationToken).ConfigureAwait(false);
    }
}