using MarketMania.Contracts;

namespace MarketMania.Services;

public class PageService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) : IPageService
{
    public Task<string> GetPageAsync(string path, object routeValues = null, CancellationToken cancellationToken = default)
    {
        var page = linkGenerator.GetUriByPage(httpContextAccessor.HttpContext, path, null, routeValues);
        return Task.FromResult(page);
    }
}