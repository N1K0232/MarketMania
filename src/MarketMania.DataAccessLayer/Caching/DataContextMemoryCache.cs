using MarketMania.DataAccessLayer.Caching.Interfaces;
using MarketMania.DataAccessLayer.Entities.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MarketMania.DataAccessLayer.Caching;

public class DataContextMemoryCache(IMemoryCache cache, ILogger<DataContextMemoryCache> logger) : IDataContextCache
{
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Removing item(s) from the cache");
        cache.Remove(key);

        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        logger.LogInformation("Get a specified item from cache");
        var entity = cache.Get<T>(id);

        return Task.FromResult(entity);
    }

    public Task<IEnumerable<T>?> GetListAsync<T>(string key, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        logger.LogInformation("Getting a list of items from cache");
        var entities = cache.Get<IEnumerable<T>>(key);

        return Task.FromResult(entities);
    }

    public Task SetAsync<T>(T value, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        logger.LogInformation("Adding a new item in cache");
        cache.Set(value.Id.ToString(), value);

        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, IEnumerable<T> values, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        logger.LogInformation("Adding a list of items in cache");
        cache.Set(key, values);

        return Task.CompletedTask;
    }
}