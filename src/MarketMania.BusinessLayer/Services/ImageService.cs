using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.StorageProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MimeMapping;
using OperationResults;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class ImageService(IApplicationDbContext applicationDbContext, IStorageProvider storageProvider, IMapper mapper) : IImageService
{
    public async Task<Result> DeleteAsync(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        var product = await applicationDbContext.GetData<Entities.Product>().FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", $"No product found with id {productId}");
        }

        var image = await applicationDbContext.GetAsync<Entities.Image>(imageId, cancellationToken);
        if (image is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No image found", $"No image found with productId {productId} and imageId {imageId}");
        }

        await storageProvider.DeleteAsync(image.Path, cancellationToken);
        await applicationDbContext.DeleteAsync(image, cancellationToken);

        product.ImagesCount--;
        if (product.ImagesCount == 0)
        {
            product.ImageUrl = null;
        }

        await applicationDbContext.SaveAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result<Image>> GetAsync(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", $"No product found with id {productId}");
        }

        var dbImage = await applicationDbContext.GetAsync<Entities.Image>(imageId, cancellationToken);
        if (dbImage is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No image found", $"No image found with id {imageId}");
        }

        if (!dbImage.IsPublished)
        {
            return Result.Fail(FailureReasons.Forbidden, "You're not allowed to see this content", "You're not allowed to see this content");
        }

        var image = mapper.Map<Image>(dbImage);
        return image;
    }

    public async Task<Result<PaginatedList<Image>>> GetListAsync(Guid productId, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.GetData<Entities.Image>()
            .Where(i => i.ProductId == productId && i.IsPublished);

        var totalCount = await query.CountAsync(cancellationToken);
        var images = await query.ProjectTo<Image>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);

        var list = new PaginatedList<Image>(images, totalCount);
        return list;
    }

    public async Task<Result<StreamFileContent>> ReadAsync(Guid productId, Guid imageId, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", $"No product found with id {productId}");
        }

        var image = await applicationDbContext.GetAsync<Entities.Image>(imageId, cancellationToken);
        if (image is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No image found", $"No image found with id {imageId}");
        }

        if (!image.IsPublished)
        {
            return Result.Fail(FailureReasons.Forbidden, "You're not allowed to see this content", "You're not allowed to see this content");
        }

        var stream = await storageProvider.ReadAsync(image.Path, cancellationToken);
        if (stream is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No stream uploaded", "No stream uploaded");
        }

        var streamFileContent = new StreamFileContent(stream, image.ContentType);
        return streamFileContent;
    }

    public async Task<Result<Image>> UploadAsync(Guid productId, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();
        var product = await applicationDbContext.GetData<Entities.Product>(true).FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Invalid product", $"No product found with id {productId}");
        }

        var path = $"products\\{productId}\\{file.FileName}";
        if (await storageProvider.ExistsAsync(path, cancellationToken))
        {
            return Result.Fail(FailureReasons.Conflict, "This image was already uploaded", "This image was already uploaded");
        }

        await storageProvider.SaveAsync(path, stream, cancellationToken);

        if (product.ImagesCount == 0)
        {
            product.ImageUrl = path;
            product.ImagesCount++;
        }

        var dbImage = new Entities.Image
        {
            ProductId = productId,
            Path = path,
            ContentType = MimeUtility.GetMimeMapping(file.FileName),
            Length = stream.Length,
        };

        await applicationDbContext.InsertAsync(dbImage, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        var savedImage = mapper.Map<Image>(dbImage);
        return savedImage;
    }
}