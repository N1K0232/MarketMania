using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface ISpecificationService
{
    Task<Result> DeleteAsync(Guid productId, Guid specificationId, CancellationToken cancellationToken);

    Task<Result<Specification>> GetAsync(Guid productId, Guid specificationId, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Specification>>> GetListAsync(Guid productId, string name, CancellationToken cancellationToken);

    Task<Result<Specification>> InsertAsync(Guid productId, SaveSpecificationRequest request, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid productId, Guid specificationId, SaveSpecificationRequest request, CancellationToken cancellationToken);
}