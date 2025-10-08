using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class PromotionEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var promotionApiGroup = endpoints.MapGroup("/api/promotions").WithTags("Promotions");

        promotionApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeletePromotion")
            .WithOpenApi();

        promotionApiGroup.MapGet("{id:guid}", GetAsync)
            .AllowAnonymous()
            .Produces<Category>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetPromotion")
            .WithOpenApi();

        promotionApiGroup.MapGet(string.Empty, GetListAsync)
            .AllowAnonymous()
            .Produces<IEnumerable<Promotion>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetPromotions")
            .WithOpenApi();

        promotionApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SavePromotionRequest>()
            .Produces<Promotion>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .WithName("InsertPromotion")
            .WithOpenApi();

        promotionApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("Admin")
            .WithValidation<SavePromotionRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UpdatePromotion")
            .WithOpenApi();
    }

    public static async Task<IResult> DeleteAsync(Guid id, IPromotionService promotionService, HttpContext httpContext)
    {
        var result = await promotionService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetAsync(Guid id, IPromotionService promotionService, HttpContext httpContext)
    {
        var result = await promotionService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetListAsync(string name, IPromotionService promotionService, HttpContext httpContext)
    {
        var result = await promotionService.GetListAsync(name, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> InsertAsync(SavePromotionRequest request, IPromotionService promotionService, HttpContext httpContext)
    {
        var result = await promotionService.InsertAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetCategory", new { id = result.Content?.Id });
        return response;
    }

    public static async Task<IResult> UpdateAsync(Guid id, SavePromotionRequest request, IPromotionService promotionService, HttpContext httpContext)
    {
        var result = await promotionService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}