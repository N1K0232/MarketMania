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

    Task SetAsync<T>(string key, IEnumerable<T> values, CancellationToken cancellationToken = default) where T : BaseEntity;
}