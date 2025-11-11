using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class BrandService(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<BrandService> logger) : IBrandService
{
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var brand = await applicationDbContext.GetAsync<Entities.Brand>(id, cancellationToken);
            if (brand is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "No brand found", $"No brand found with id {id}");
            }

            await applicationDbContext.DeleteAsync(brand, cancellationToken);
            await applicationDbContext.SaveAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to delete the brand", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to delete the brand", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No brand found", ex.Message);
        }
    }

    public async Task<Result<Brand>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbBrand = await applicationDbContext.GetAsync<Entities.Brand>(id, cancellationToken);
        if (dbBrand is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No brand found", $"No brand found with id {id}");
        }

        var brand = mapper.Map<Brand>(dbBrand);
        return brand;
    }

    public async Task<Result<IEnumerable<Brand>>> GetListAsync(string? name, CancellationToken cancellationToken)
    {
        var brands = await applicationDbContext.GetData<Entities.Brand>()
            .WhereIf(name.HasValue(), b => b.Name.Contains(name!))
            .ProjectTo<Brand>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return brands;
    }

    public async Task<Result<Brand>> InsertAsync(SaveBrandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var brandExists = await applicationDbContext.GetData<Entities.Brand>().AnyAsync(b => b.Name == request.Name && b.City == request.City, cancellationToken);
            if (brandExists)
            {
                return Result.Fail(FailureReasons.Conflict, "Brand already exists", $"Brand {request.Name} already exists");
            }

            var dbBrand = mapper.Map<Entities.Brand>(request);
            await applicationDbContext.InsertAsync(dbBrand, cancellationToken);
            await applicationDbContext.SaveAsync(cancellationToken);

            var savedBrand = mapper.Map<Brand>(dbBrand);
            return savedBrand;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to create the brand", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to create the brand", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(Guid id, SaveBrandRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dbBrand = await applicationDbContext.GetData<Entities.Brand>(true).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            if (dbBrand is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "No brand found", $"No brand found with id {id}");
            }

            mapper.Map(request, dbBrand);

            await applicationDbContext.SaveAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to update the brand", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to update the brand", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No brand found", ex.Message);
        }
    }
}