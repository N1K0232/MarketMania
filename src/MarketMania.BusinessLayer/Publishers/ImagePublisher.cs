using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Entities;
using MarketMania.Shared.Models.Notifications;
using MarketMania.StorageProviders;
using Microsoft.EntityFrameworkCore;
using SimpleTransit;

namespace MarketMania.BusinessLayer.Publishers;

public class ImagePublisher(IApplicationDbContext applicationDbContext, IStorageProvider storageProvider) : INotificationHandler<ImageCreated>, INotificationHandler<ImageDeleted>
{
    public async Task HandleAsync(ImageCreated message, CancellationToken cancellationToken)
    {
        if (await storageProvider.ExistsAsync(message.Path, cancellationToken).ConfigureAwait(false))
        {
            throw new IOException($"The image {message.Path} was already uploaded");
        }

        var product = await applicationDbContext.GetData<Product>(true).FirstAsync(p => p.Id == message.ProductId, cancellationToken).ConfigureAwait(false);
        await storageProvider.SaveAsync(message.Path, message.Stream, cancellationToken).ConfigureAwait(false);

        if (product.ImagesCount == 0)
        {
            product.ImageUrl = message.Path;
        }

        product.ImagesCount++;
        await applicationDbContext.SaveAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task HandleAsync(ImageDeleted message, CancellationToken cancellationToken)
    {
        var product = await applicationDbContext.GetData<Product>(true).FirstAsync(p => p.Id == message.ProductId, cancellationToken).ConfigureAwait(false);
        await storageProvider.DeleteAsync(message.Path, cancellationToken).ConfigureAwait(false);

        if (product.ImagesCount == 0)
        {
            product.ImageUrl = null;
        }

        product.ImagesCount--;
        await applicationDbContext.SaveAsync(cancellationToken).ConfigureAwait(false);
    }
}