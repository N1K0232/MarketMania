using MarketMania.Shared.Models;
using OperationResults;

namespace MarketMania.BusinessLayer.Services.Interfaces;

public interface IImageService
{
    Task<Result> DeleteAsync(Guid productId, Guid imageId, CancellationToken cancellationToken);

    Task<Result<Image>> GetAsync(Guid productId, Guid imageId, CancellationToken cancellationToken);

    Task<Result<PaginatedList<Image>>> GetListAsync(Guid productId, CancellationToken cancellationToken);

    Task<Result<StreamFileContent>> ReadAsync(Guid productId, Guid imageId, CancellationToken cancellationToken);

    Task<Result<Image>> UploadAsync(Guid productId, Stream stream, string fileName, CancellationToken cancellationToken);
}