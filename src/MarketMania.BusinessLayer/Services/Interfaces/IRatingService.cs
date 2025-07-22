using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface IRatingService
{
    Task<Result> DeleteAsync(Guid productId, Guid ratingId, CancellationToken cancellationToken);

    Task<Result<Rating>> GetAsync(Guid productId, Guid ratingId, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Rating>>> GetListAsync(Guid productId, CancellationToken cancellationToken);

    Task<Result<Rating>> PublishAsync(Guid productId, NewRatingRequest request, CancellationToken cancellationToken);
}