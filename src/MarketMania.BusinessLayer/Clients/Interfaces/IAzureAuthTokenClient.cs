namespace MarketMania.BusinessLayer.Clients.Interfaces;

public interface IAzureAuthTokenClient
{
    string Region { get; set; }

    Uri ServiceUrl { get; set; }

    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}