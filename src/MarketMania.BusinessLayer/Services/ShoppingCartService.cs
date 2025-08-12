using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class ShoppingCartService(IApplicationDbContext applicationDbContext, HybridCache cache)
{
    private async Task SaveCartAsync(Guid userId, ShoppingCartRequest request, CancellationToken cancellationToken)
    {
        var existingCart = await applicationDbContext.GetData<Entities.ShoppingCart>()
            .Include(s => s.ShoppingCartItems)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        if (existingCart is null)
        {
            existingCart = new Entities.ShoppingCart
            {
                UserId = userId
            };

            await applicationDbContext.InsertAsync(existingCart, cancellationToken);
        }
        else
        {

        }

        await applicationDbContext.SaveAsync(cancellationToken);
    }
}