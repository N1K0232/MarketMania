namespace MarketMania.Shared.Models;

public class Promotion
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}