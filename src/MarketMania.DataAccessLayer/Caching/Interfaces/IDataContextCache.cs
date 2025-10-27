using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer.Caching.Interfaces;

public interface IDataContextCache
{
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity;

    Task<IEnumerable<T>?> GetListAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(T value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default) where T : BaseEntity;
}