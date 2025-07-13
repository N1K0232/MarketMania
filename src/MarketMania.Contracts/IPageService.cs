namespace MarketMania.Contracts;

public interface IPageService
{
    Task<string> GetPageAsync(string path, object routeValues = null, CancellationToken cancellationToken = default);
}