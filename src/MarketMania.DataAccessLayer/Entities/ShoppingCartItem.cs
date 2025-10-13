using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Entities;

public class ShoppingCartItem : BaseEntity
{
    public Guid ShoppingCartId { get; set; }

    public Guid ProductId { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public virtual ShoppingCart ShoppingCart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}