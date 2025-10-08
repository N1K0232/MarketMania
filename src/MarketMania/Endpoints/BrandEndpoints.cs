using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class BrandEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var brandsApiGroup = endpoints.MapGroup("/api/brands").WithTags("Brands");

        brandsApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteBrand")
            .WithOpenApi();

        brandsApiGroup.MapGet("{id:guid}", GetAsync)
            .AllowAnonymous()
            .Produces<Brand>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetBrand")
            .WithOpenApi();

        brandsApiGroup.MapGet(string.Empty, GetListAsync)
            .AllowAnonymous()
            .Produces<IEnumerable<Category>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetBrands")
            .WithOpenApi();

        brandsApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveBrandRequest>()
            .Produces<Category>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .WithName("InsertBrand")
            .WithOpenApi();

        brandsApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveBrandRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UpdateBrand")
            .WithOpenApi();
    }

    public static async Task<IResult> DeleteAsync(Guid id, IBrandService brandService, HttpContext httpContext)
    {
        var result = await brandService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetAsync(Guid id, IBrandService brandService, HttpContext httpContext)
    {
        var result = await brandService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetListAsync(string name, IBrandService brandService, HttpContext httpContext)
    {
        var result = await brandService.GetListAsync(name, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> InsertAsync(SaveBrandRequest request, IBrandService brandService, HttpContext httpContext)
    {
        var result = await brandService.InsertAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetBrand", new { id = result.Content?.Id });
        return response;
    }

    public static async Task<IResult> UpdateAsync(Guid id, SaveBrandRequest request, IBrandService brandService, HttpContext httpContext)
    {
        var result = await brandService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}