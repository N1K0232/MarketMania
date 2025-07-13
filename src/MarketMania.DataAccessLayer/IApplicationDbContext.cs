using MarketMania.DataAccessLayer.Entities.Common;

namespace MarketMania.DataAccessLayer;

public interface IApplicationDbContext
{
    Task DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity;

    Task DeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : BaseEntity;

    ValueTask<T> GetAsync<T>(Guid id, CancellationToken cancellationToken) where T : BaseEntity;

    IQueryable<T> GetData<T>(bool trackingChanges = false) where T : BaseEntity;

    Task InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity;

    Task SaveAsync(CancellationToken cancellationToken);

    Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
}