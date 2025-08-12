namespace MarketMania.Shared.Models;

public class ShoppingCartItem
{
    public string Product { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
}