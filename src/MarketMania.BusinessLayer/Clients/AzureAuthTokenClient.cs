using MarketMania.BusinessLayer.Clients.Interfaces;
using MarketMania.BusinessLayer.Exceptions;
using MarketMania.BusinessLayer.Resources;
using Microsoft.Extensions.Caching.Memory;

namespace MarketMania.BusinessLayer.Clients;

public class AzureAuthTokenClient(HttpClient httpClient, IMemoryCache cache) : IAzureAuthTokenClient
{
    public Uri ServiceUrl { get; set; }

    public string Region { get; set; }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue<string>("access-token", out var cachedToken))
        {
            return cachedToken;
        }

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, ServiceUrl);
            httpRequest.Headers.Add(Constants.OcpApimSubscriptionKeyHeader, "");
            httpRequest.Headers.Add(Constants.OcpApimSubscriptionRegionHeader, Region);

            using var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);
            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                var token = $"Bearer {content}";

                return cache.Set("access-token", token);
            }

            throw await TranslatorClientException.ReadFromResponseAsync(httpResponse, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new TranslatorClientException(500, ex.Message);
        }
    }
}