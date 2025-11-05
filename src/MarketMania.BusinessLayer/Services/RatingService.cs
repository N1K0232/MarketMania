using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Clients.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using SimpleAuthentication;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class RatingService(IApplicationDbContext applicationDbContext, ISentimentAnalysisClient sentimentAnalysisClient, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IRatingService
{
    public async Task<Result> DeleteAsync(Guid productId, Guid ratingId, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>(true).AnyAsync(p => p.Id == productId && p.IsPublished, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", $"No product found with id {productId}");
        }

        var rating = await applicationDbContext.GetAsync<Entities.Rating>(ratingId, cancellationToken);
        if (rating is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No rating found", $"No rating found with id {ratingId}");
        }

        await applicationDbContext.DeleteAsync(rating, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<Rating>> GetAsync(Guid productId, Guid ratingId, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId && p.IsPublished, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", $"No product found with id {productId}");
        }

        var dbRating = await applicationDbContext.GetAsync<Entities.Rating>(ratingId, cancellationToken);
        if (dbRating is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No rating found", $"No rating found with id {ratingId}");
        }

        if (!dbRating.IsPublished)
        {
            return Result.Fail(FailureReasons.Forbidden, "You're not allowed to see this rating", "You're not allowed to see this rating");
        }

        var rating = mapper.Map<Rating>(dbRating);
        return rating;
    }

    public async Task<Result<IEnumerable<Rating>>> GetListAsync(Guid productId, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId && p.IsPublished, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", $"No product found with id {productId}");
        }

        var ratings = await applicationDbContext.GetData<Entities.Rating>()
            .Where(r => r.ProductId == productId && r.IsPublished)
            .ProjectTo<Rating>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return ratings;
    }

    public async Task<Result<Rating>> PublishAsync(Guid productId, NewRatingRequest request, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>(true).AnyAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", $"No product found with id {productId}");
        }

        var dbRating = mapper.Map<Entities.Rating>(request);
        dbRating.ProductId = productId;
        dbRating.UserId = Guid.Parse(httpContextAccessor.HttpContext?.User.GetClaimValue(ClaimTypes.NameIdentifier)!);

        var sentimentResponse = await sentimentAnalysisClient.GetPredictionAsync(request.Text, cancellationToken);
        dbRating.SentimentScore = sentimentResponse!.Score;

        await applicationDbContext.InsertAsync(dbRating, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        var savedRating = mapper.Map<Rating>(dbRating);
        return savedRating;
    }
}