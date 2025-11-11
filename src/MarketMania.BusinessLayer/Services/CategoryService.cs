using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class CategoryService(IApplicationDbContext applicationDbContext, IMapper mapper) : ICategoryService
{
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await applicationDbContext.GetAsync<Entities.Category>(id, cancellationToken);
            if (category is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "Category not found", $"No category found for id {id}");
            }

            await applicationDbContext.DeleteAsync(category, cancellationToken);
            await applicationDbContext.SaveAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to delete the category", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to delete the category", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No category found", ex.Message);
        }
    }

    public async Task<Result<Category>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbCategory = await applicationDbContext.GetAsync<Entities.Category>(id, cancellationToken);
        if (dbCategory is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Category not found", $"No category found for id {id}");
        }

        var category = mapper.Map<Category>(dbCategory);
        return category;
    }

    public async Task<Result<IEnumerable<Category>>> GetListAsync(string? name, CancellationToken cancellationToken)
    {
        var categories = await applicationDbContext.GetData<Entities.Category>()
            .WhereIf(name.HasValue(), c => c.Name.Contains(name!))
            .ProjectTo<Category>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return categories;
    }

    public async Task<Result<Category>> InsertAsync(SaveCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await applicationDbContext.GetData<Entities.Category>().AnyAsync(c => c.Name == request.Name && c.Description == request.Description, cancellationToken);
            if (exists)
            {
                return Result.Fail(FailureReasons.Conflict, "Category already exists", $"The category {request.Name} already exists");
            }

            var dbCategory = mapper.Map<Entities.Category>(request);
            await applicationDbContext.InsertAsync(dbCategory, cancellationToken);
            await applicationDbContext.SaveAsync(cancellationToken);

            var savedCategory = mapper.Map<Category>(dbCategory);
            return savedCategory;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to create the category", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to create the category", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(Guid id, SaveCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dbCategory = await applicationDbContext.GetAsync<Entities.Category>(id, cancellationToken);
            if (dbCategory is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "Category not found", $"No category found for id {id}");
            }

            mapper.Map(request, dbCategory);

            await applicationDbContext.SaveAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to update the category", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to update the category", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No category found", ex.Message);
        }
    }
}