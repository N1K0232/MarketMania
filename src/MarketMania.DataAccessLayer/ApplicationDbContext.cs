using System.Data;
using System.Reflection;
using EntityFramework.Exceptions.SqlServer;
using MarketMania.Authentication;
using MarketMania.DataAccessLayer.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.DataAccessLayer;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : AuthenticationDbContext(options), IApplicationDbContext
{
    public Task DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity
    {
        Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : BaseEntity
    {
        Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async ValueTask<T?> GetAsync<T>(Guid id, CancellationToken cancellationToken) where T : BaseEntity
    {
        var entity = await Set<T>().FindAsync([id], cancellationToken).ConfigureAwait(false);
        return entity;
    }

    public IQueryable<T> GetData<T>(bool trackingChanges = false) where T : BaseEntity
    {
        var set = Set<T>();
        return trackingChanges ? set.AsTracking() : set.AsNoTrackingWithIdentityResolution();
    }

    public async Task InsertAsync<T>(T entity, CancellationToken cancellationToken) where T : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        await Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.Entity.GetType()))
            .ToList();

        foreach (var entry in entries.Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            var entity = entry.Entity as BaseEntity;

            if (entry.State is EntityState.Modified)
            {
                entity!.LastModifiedAt = DateTime.UtcNow;
            }
        }

        await SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
    }

    public async Task ExecuteTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        var strategy = Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync((token) => ExecuteTransactionInternalAsync(action, token), cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> ExecuteTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync((token) => ExecuteTransactionInternalAsync(action, token), cancellationToken).ConfigureAwait(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
        optionsBuilder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    private async Task ExecuteTransactionInternalAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await action.Invoke(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    private async Task<T> ExecuteTransactionInternalAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await action.Invoke(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }
}