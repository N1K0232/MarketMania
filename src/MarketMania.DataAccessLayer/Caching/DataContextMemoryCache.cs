using MarketMania.DataAccessLayer.Caching.Interfaces;
using MarketMania.DataAccessLayer.Entities.Common;
using Microsoft.Extensions.Caching.Memory;

namespace MarketMania.DataAccessLayer.Caching;

public class DataContextMemoryCache(IMemoryCache cache) : IDataContextCache
{
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cache.Remove(id);
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var result = cache.Get<T>(id);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>?> GetListAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var result = cache.Get<IEnumerable<T>>(key);
        return Task.FromResult(result);
    }

    public Task SetAsync<T>(T value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        cache.Set(value.Id, value, absoluteExpirationRelativeToNow);
        return Task.CompletedTask;
    }
}