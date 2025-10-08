using MarketMania.Contracts;

namespace MarketMania.Services;

public class PageService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : IPageService
{
    public Task<string> GetPageAsync(string path, object routeValues = null, CancellationToken cancellationToken = default)
    {
        var page = linkGenerator.GetUriByName(httpContextAccessor.HttpContext, "ConfirmEmail", routeValues);
        return Task.FromResult(page);
    }

    public Task<string> GetEndpointAsync(string endpointName, object routeValues = null, CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var endpoint = linkGenerator.GetUriByName(httpContext, endpointName, routeValues, httpContext.Request.Scheme, httpContext.Request.Host);

        return Task.FromResult(endpoint);
    }
}