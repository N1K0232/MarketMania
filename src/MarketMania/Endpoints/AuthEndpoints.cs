using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class AuthEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var authApiGroup = endpoints.MapGroup("/api/auth").AllowAnonymous();

        authApiGroup.MapPost("login", LoginAsync)
            .Produces<AuthResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("login")
            .WithOpenApi();

        authApiGroup.MapPost("register", RegisterAsync)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("register")
            .WithOpenApi();
    }

    public static async Task<IResult> LoginAsync(LoginRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.LoginAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> RegisterAsync(RegisterRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.RegisterAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, StatusCodes.Status201Created);
        return response;
    }
}