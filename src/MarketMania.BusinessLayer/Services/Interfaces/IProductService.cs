using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services;

public interface IProductService
{
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Product>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<PaginatedList<Product>>> GetListAsync(SearchProductRequest request, CancellationToken cancellationToken);

    Task<Result<Product>> InsertAsync(SaveProductRequest request, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveProductRequest request, CancellationToken cancellationToken);
}