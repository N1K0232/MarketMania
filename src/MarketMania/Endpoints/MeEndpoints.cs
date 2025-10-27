using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class MeEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var meApiGroup = endpoints.MapGroup("/api/me").WithTags("Me").RequireAuthorization();

        meApiGroup.MapGet("/api/me", GetAsync)
            .Produces<User>()
            .WithName("me")
            .WithOpenApi();
    }

    public static async Task<IResult> GetAsync(IMeService meService, HttpContext httpContext)
    {
        var result = await meService.GetAsync();

        var response = httpContext.CreateResponse(result);
        return response;
    }
}