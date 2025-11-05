using System.Text.Json;
using MarketMania.DataAccessLayer.Caching.Interfaces;
using MarketMania.DataAccessLayer.Entities.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace MarketMania.DataAccessLayer.Caching;

public class DataContextDistributedCache(IDistributedCache cache) : IDataContextCache
{
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => cache.RemoveAsync(key, cancellationToken);

    public async Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var entity = await cache.GetStringAsync(id.ToString(), cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(entity);
    }

    public async Task<IEnumerable<T>?> GetListAsync<T>(string key, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var entities = await cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
        if (entities is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<IEnumerable<T>>(entities);
    }

    public Task SetAsync<T>(T value, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        var content = JsonSerializer.Serialize(value);
        return cache.SetStringAsync(value.Id.ToString(), content, cancellationToken);
    }
}