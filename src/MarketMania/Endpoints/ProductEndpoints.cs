using MarketMania.BusinessLayer.Services;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class ProductEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var productsApiGroup = endpoints.MapGroup("/api/products");

        productsApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteProduct")
            .WithOpenApi();

        productsApiGroup.MapGet("{id:guid}", GetAsync)
            .AllowAnonymous()
            .Produces<Product>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetProduct")
            .WithOpenApi();

        productsApiGroup.MapGet(string.Empty, GetListAsync)
            .AllowAnonymous()
            .Produces<PaginatedList<Product>>()
            .WithName("GetProducts")
            .WithOpenApi();

        productsApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveProductRequest>()
            .Produces<Product>(StatusCodes.Status201Created)
            .WithName("InsertProduct")
            .WithOpenApi();

        productsApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveProductRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UpdateProduct")
            .WithOpenApi();
    }

    public static async Task<IResult> DeleteAsync(Guid id, IProductService productService, HttpContext httpContext)
    {
        var result = await productService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetAsync(Guid id, IProductService productService, HttpContext httpContext)
    {
        var result = await productService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetListAsync(string name, string brand, string category, int pageIndex, int itemsPerPage, string orderBy, IProductService productService, HttpContext httpContext)
    {
        var result = await productService.GetListAsync(name, brand, category, pageIndex, itemsPerPage, orderBy, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> InsertAsync(SaveProductRequest request, IProductService productService, HttpContext httpContext)
    {
        var result = await productService.InsertAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetProduct", new { id = result.Content?.Id });
        return response;
    }

    public static async Task<IResult> UpdateAsync(Guid id, SaveProductRequest request, IProductService productService, HttpContext httpContext)
    {
        var result = await productService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}