using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer;

public interface IApplicationDbContext
{
    Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity;

    Task DeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : BaseEntity;

    ValueTask<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : BaseEntity;

    IQueryable<T> GetData<T>(bool trackingChanges = false) where T : BaseEntity;

    Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity;

    Task<int> SaveAsync(CancellationToken cancellationToken = default);

    Task ExecuteTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);

    Task<T> ExecuteTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}