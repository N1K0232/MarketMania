using MarketMania.Authentication.Entities;
using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class ShoppingCart : BaseEntity
{
    public Guid UserId { get; set; }

    public virtual ApplicationUser User { get; set; }

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
}