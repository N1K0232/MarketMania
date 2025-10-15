using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface IPromotionService
{
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<Promotion>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Promotion>>> GetListAsync(string? name, CancellationToken cancellationToken);

    Task<Result<Promotion>> InsertAsync(SavePromotionRequest request, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SavePromotionRequest request, CancellationToken cancellationToken);
}