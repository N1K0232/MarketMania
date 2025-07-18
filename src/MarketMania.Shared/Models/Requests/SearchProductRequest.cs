namespace MarketMania.Shared.Models.Requests;

public record class SearchProductRequest(string Name, string Brand, string Category, int PageIndex = 0, int ItemsPerPage = 50, string OrderBy = "Name, Price");