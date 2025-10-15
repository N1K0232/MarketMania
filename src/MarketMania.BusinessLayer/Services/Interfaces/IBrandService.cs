using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface IBrandService
{
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Brand>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Brand>>> GetListAsync(string? name, CancellationToken cancellationToken);

    Task<Result<Brand>> InsertAsync(SaveBrandRequest request, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveBrandRequest request, CancellationToken cancellationToken);
}