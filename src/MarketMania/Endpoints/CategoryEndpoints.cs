using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class CategoryEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var categoriesApiGroup = endpoints.MapGroup("/api/categories").WithTags("Categories");

        categoriesApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteCategory")
            .WithOpenApi();

        categoriesApiGroup.MapGet("{id:guid}", GetAsync)
            .AllowAnonymous()
            .Produces<Category>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetCategory")
            .WithOpenApi();

        categoriesApiGroup.MapGet(string.Empty, GetListAsync)
            .AllowAnonymous()
            .Produces<IEnumerable<Category>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetCategories")
            .WithOpenApi();

        categoriesApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveCategoryRequest>()
            .Produces<Category>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .WithName("InsertCategory")
            .WithOpenApi();

        categoriesApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveCategoryRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UpdateCategory")
            .WithOpenApi();
    }

    public static async Task<IResult> DeleteAsync(Guid id, ICategoryService categoryService, HttpContext httpContext)
    {
        var result = await categoryService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetAsync(Guid id, ICategoryService categoryService, HttpContext httpContext)
    {
        var result = await categoryService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetListAsync(string? name, ICategoryService categoryService, HttpContext httpContext)
    {
        var result = await categoryService.GetListAsync(name, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> InsertAsync(SaveCategoryRequest request, ICategoryService categoryService, HttpContext httpContext)
    {
        var result = await categoryService.InsertAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetCategory", new { id = result.Content?.Id });
        return response;
    }

    public static async Task<IResult> UpdateAsync(Guid id, SaveCategoryRequest request, ICategoryService categoryService, HttpContext httpContext)
    {
        var result = await categoryService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}