using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services;

public interface IProductService
{
    Task<Result> ConfirmAsync(Guid id, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Product>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<PaginatedList<Product>>> GetListAsync(string name, string brand, string category, int pageIndex, int itemsPerPage, string orderBy, CancellationToken cancellationToken);

    Task<Result<Product>> InsertAsync(SaveProductRequest request, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveProductRequest request, CancellationToken cancellationToken);
}