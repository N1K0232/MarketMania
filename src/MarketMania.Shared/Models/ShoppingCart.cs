namespace MarketMania.Shared.Models;

public record class ShoppingCart(Guid Id, Guid UserId, DateTime CreatedAt, ICollection<ShoppingCartItem> Items);