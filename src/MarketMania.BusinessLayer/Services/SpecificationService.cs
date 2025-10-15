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

public class SpecificationService(IApplicationDbContext applicationDbContext, IMapper mapper) : ISpecificationService
{
    public async Task<Result> DeleteAsync(Guid productId, Guid specificationId, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {productId}");
        }

        var specification = await applicationDbContext.GetAsync<Entities.Specification>(specificationId, cancellationToken);
        if (specification is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Specification not found", $"No specification found with id {specificationId}");
        }

        await applicationDbContext.DeleteAsync(specification, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<Specification>> GetAsync(Guid productId, Guid specificationId, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {productId}");
        }

        var specification = await applicationDbContext.GetData<Entities.Specification>()
            .Where(s => s.Id == specificationId)
            .ProjectTo<Specification>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (specification is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Specification not found", $"No specification found with id {specificationId}");
        }

        return specification;
    }

    public async Task<Result<IEnumerable<Specification>>> GetListAsync(Guid productId, string? name, CancellationToken cancellationToken)
    {
        var specifications = await applicationDbContext.GetData<Entities.Specification>()
            .Where(s => s.ProductId == productId)
            .WhereIf(name.HasValue(), s => s.Name.Contains(name!))
            .ProjectTo<Specification>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return specifications;
    }

    public async Task<Result<Specification>> InsertAsync(Guid productId, SaveSpecificationRequest request, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {productId}");
        }

        var dbSpecification = mapper.Map<Entities.Specification>(request);
        dbSpecification.ProductId = productId;

        await applicationDbContext.InsertAsync(dbSpecification, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        var savedSpecification = mapper.Map<Specification>(dbSpecification);
        return savedSpecification;
    }

    public async Task<Result> UpdateAsync(Guid productId, Guid specificationId, SaveSpecificationRequest request, CancellationToken cancellationToken)
    {
        var productExists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Id == productId, cancellationToken);
        if (!productExists)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {productId}");
        }

        var specification = await applicationDbContext.GetData<Entities.Specification>(true)
            .FirstOrDefaultAsync(s => s.Id == specificationId, cancellationToken);

        if (specification is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Specification not found", $"No specification found with id {specificationId}");
        }

        mapper.Map(request, specification);
        await applicationDbContext.SaveAsync(cancellationToken);

        return Result.Ok();
    }
}