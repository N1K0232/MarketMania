using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class SupplierEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var suppliersApiGroup = endpoints.MapGroup("/api/suppliers").WithTags("Suppliers");

        suppliersApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteSupplier")
            .WithOpenApi();

        suppliersApiGroup.MapGet("{id:guid}", GetAsync)
            .AllowAnonymous()
            .Produces<Supplier>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetSupplier")
            .WithOpenApi();

        suppliersApiGroup.MapGet(string.Empty, GetListAsync)
            .AllowAnonymous()
            .Produces<IEnumerable<Supplier>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetSuppliers")
            .WithOpenApi();

        suppliersApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveSupplierRequest>()
            .Produces<Supplier>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .WithName("InsertSupplier")
            .WithOpenApi();

        suppliersApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SaveSupplierRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UpdateSupplier")
            .WithOpenApi();
    }

    public static async Task<IResult> DeleteAsync(Guid id, ISupplierService supplierService, HttpContext httpContext)
    {
        var result = await supplierService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetAsync(Guid id, ISupplierService supplierService, HttpContext httpContext)
    {
        var result = await supplierService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetListAsync(string? name, ISupplierService supplierService, HttpContext httpContext)
    {
        var result = await supplierService.GetListAsync(name, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> InsertAsync(SaveSupplierRequest request, ISupplierService supplierService, HttpContext httpContext)
    {
        var result = await supplierService.InsertAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetCategory", new { id = result.Content?.Id });
        return response;
    }

    public static async Task<IResult> UpdateAsync(Guid id, SaveSupplierRequest request, ISupplierService supplierService, HttpContext httpContext)
    {
        var result = await supplierService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}