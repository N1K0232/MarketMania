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

public class SupplierService(IApplicationDbContext applicationDbContext, IMapper mapper) : ISupplierService
{
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var supplier = await applicationDbContext.GetAsync<Entities.Supplier>(id, cancellationToken);
        if (supplier is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No supplier found", $"No supplier found with id {id}");
        }

        await applicationDbContext.DeleteAsync(supplier, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<Supplier>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbSupplier = await applicationDbContext.GetAsync<Entities.Supplier>(id, cancellationToken);
        if (dbSupplier is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No supplier found", $"No supplier found with id {id}");
        }

        var supplier = mapper.Map<Supplier>(dbSupplier);
        return supplier;
    }

    public async Task<Result<IEnumerable<Supplier>>> GetListAsync(string name, CancellationToken cancellationToken)
    {
        var suppliers = await applicationDbContext.GetData<Entities.Supplier>()
            .WhereIf(name.HasValue(), s => s.Name.Contains(name))
            .ProjectTo<Supplier>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return suppliers;
    }

    public async Task<Result<Supplier>> InsertAsync(SaveSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplierExists = await applicationDbContext.GetData<Entities.Supplier>().AnyAsync(s => s.Name == request.Name && s.City == request.City, cancellationToken);
        if (supplierExists)
        {
            return Result.Fail(FailureReasons.Conflict, "Supplier already exists", $"Brand {request.Name} already exists");
        }

        var dbSupplier = mapper.Map<Entities.Supplier>(request);
        await applicationDbContext.InsertAsync(dbSupplier, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        var savedSupplier = mapper.Map<Supplier>(request);
        return savedSupplier;
    }

    public async Task<Result> UpdateAsync(Guid id, SaveSupplierRequest request, CancellationToken cancellationToken)
    {
        var dbSupplier = await applicationDbContext.GetData<Entities.Supplier>(true).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (dbSupplier is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No supplier found", $"No supplier found with id {id}");
        }

        mapper.Map(request, dbSupplier);

        await applicationDbContext.SaveAsync(cancellationToken);
        return Result.Ok();
    }
}