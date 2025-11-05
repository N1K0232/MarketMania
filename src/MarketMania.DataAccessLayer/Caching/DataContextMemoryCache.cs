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

    public Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var entity = cache.Get<T>(id.ToString());
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<T>?> GetListAsync<T>(string key, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var entities = cache.Get<IEnumerable<T>>(key);
        return Task.FromResult(entities);
    }

    public Task SetAsync<T>(T value, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        cache.Set(value.Id.ToString(), value);
        return Task.CompletedTask;
    }
}