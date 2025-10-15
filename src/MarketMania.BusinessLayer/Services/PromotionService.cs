using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class PromotionService(IApplicationDbContext applicationDbContext, IMapper mapper) : IPromotionService
{
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await applicationDbContext.GetAsync<Entities.Promotion>(id, cancellationToken);
        if (promotion is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No promotion found", $"No promotion found with id {id}");
        }

        await applicationDbContext.DeleteAsync(promotion, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<Promotion>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbPromotion = await applicationDbContext.GetAsync<Entities.Promotion>(id, cancellationToken);
        if (dbPromotion is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No promotion found", $"No promotion found with id {id}");
        }

        var promotion = mapper.Map<Promotion>(dbPromotion);
        return promotion;
    }

    public async Task<Result<IEnumerable<Promotion>>> GetListAsync(string? name, CancellationToken cancellationToken)
    {
        var promotions = await applicationDbContext.GetData<Entities.Promotion>()
            .WhereIf(name.HasValue(), p => p.Name.Contains(name!))
            .ProjectTo<Promotion>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return promotions;
    }

    public async Task<Result<Promotion>> InsertAsync(SavePromotionRequest request, CancellationToken cancellationToken)
    {
        var promotionExists = await applicationDbContext.GetData<Entities.Promotion>().AnyAsync(p => p.Name == request.Name, cancellationToken);
        if (promotionExists)
        {
            return Result.Fail(FailureReasons.Conflict, "Promotion already exists", $"Promotion {request.Name} already exists");
        }

        var dbPromotion = mapper.Map<Entities.Promotion>(request);
        await applicationDbContext.InsertAsync(dbPromotion, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        var savedPromotion = mapper.Map<Promotion>(dbPromotion);
        return savedPromotion;
    }

    public async Task<Result> UpdateAsync(Guid id, SavePromotionRequest request, CancellationToken cancellationToken)
    {
        var dbPromotion = await applicationDbContext.GetData<Entities.Promotion>(true).FirstOrDefaultAsync(p => p.Id == id);
        if (dbPromotion is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No promotion found", $"No promotion found with id {id}");
        }

        mapper.Map(request, dbPromotion);
        await applicationDbContext.SaveAsync(cancellationToken);

        return Result.Ok();
    }
}