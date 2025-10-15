using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface ICategoryService
{
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Category>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Category>>> GetListAsync(string? name, CancellationToken cancellationToken);

    Task<Result<Category>> InsertAsync(SaveCategoryRequest request, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveCategoryRequest request, CancellationToken cancellationToken);
}