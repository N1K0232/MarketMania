namespace MarketMania.Shared.Models;

public class ShoppingCart
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<ShoppingCartItem> Items { get; set; }
}