using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface ISupplierService
{
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Supplier>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Supplier>>> GetListAsync(string? name, CancellationToken cancellationToken);

    Task<Result<Supplier>> InsertAsync(SaveSupplierRequest request, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveSupplierRequest request, CancellationToken cancellationToken);
}