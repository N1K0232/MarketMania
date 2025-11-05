using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Caching.Interfaces;

public interface IDataContextCache
{
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        => RemoveAsync(id.ToString(), cancellationToken);

    Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity;

    Task<IEnumerable<T>?> GetListAsync<T>(string key, CancellationToken cancellationToken = default) where T : BaseEntity;

    Task SetAsync<T>(T value, CancellationToken cancellationToken = default) where T : BaseEntity;

    async Task SetAsync<T>(IEnumerable<T> values, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        foreach (var value in values)
        {
            await SetAsync(value, cancellationToken).ConfigureAwait(false);
        }
    }
}