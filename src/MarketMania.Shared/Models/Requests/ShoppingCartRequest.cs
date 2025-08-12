namespace MarketMania.Shared.Models.Requests;

public record class ShoppingCartRequest(Guid ProductId, int Quantity);