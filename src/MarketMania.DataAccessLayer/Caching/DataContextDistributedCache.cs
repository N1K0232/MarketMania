using System.Text.Json;
using MarketMania.DataAccessLayer.Caching.Interfaces;
using MarketMania.DataAccessLayer.Entities.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MarketMania.DataAccessLayer.Caching;

public class DataContextDistributedCache(IDistributedCache cache, ILogger<DataContextDistributedCache> logger) : IDataContextCache
{
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("clearing cache");
            await cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while removing data from cache");
            throw;
        }
    }

    public async Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        try
        {
            logger.LogInformation("getting item from cache");
            var content = await cache.GetStringAsync(id.ToString(), cancellationToken).ConfigureAwait(false);

            if (content is null)
            {
                return null;
            }

            var entity = JsonSerializer.Deserialize<T>(content);
            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the specified element from the cache");
            throw;
        }
    }

    public async Task<IEnumerable<T>?> GetListAsync<T>(string key, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        try
        {
            logger.LogInformation("getting list of items from cache");
            var content = await cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);

            if (content is null)
            {
                return null;
            }

            var entities = JsonSerializer.Deserialize<IEnumerable<T>>(content);
            return entities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the list of items from the cache");
            throw;
        }
    }

    public async Task SetAsync<T>(T value, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        try
        {
            logger.LogInformation("storing a new item in the cache");

            var content = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(value.Id.ToString(), content, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while storing a new item in the cache");
            throw;
        }
    }

    public async Task SetAsync<T>(string key, IEnumerable<T> values, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        try
        {
            logger.LogInformation("storing a list of items in the cache");

            var content = JsonSerializer.Serialize(values);
            await cache.SetStringAsync(key, content, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while storing a new list of items in the cache");
            throw;
        }
    }
}