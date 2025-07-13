using System.Net.Mime;
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
        var authApiGroup = endpoints.MapGroup("/api/auth");

        authApiGroup.MapGet("qrcode", GetQRCodeAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Image.Png)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("qrcode")
            .WithOpenApi();

        authApiGroup.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .Produces<AuthResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("login")
            .WithOpenApi();

        authApiGroup.MapPost("logout", LogoutAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("logout")
            .WithOpenApi();

        authApiGroup.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("register")
            .WithOpenApi();

        authApiGroup.MapPost("validate2fa", ValidateTwoFactorAsync)
            .AllowAnonymous()
            .Produces<AuthResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("validate2fa")
            .WithOpenApi();

        authApiGroup.MapGet("verifyemail", VerifyEmailAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("verifyemail")
            .WithOpenApi();
    }

    public static async Task<IResult> GetQRCodeAsync(string token, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.GetQRCodeAsync(token, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> LoginAsync(LoginRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.LoginAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> LogoutAsync(IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.LogoutAsync(httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> RegisterAsync(RegisterRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.RegisterAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, StatusCodes.Status201Created);
        return response;
    }

    public static async Task<IResult> ValidateTwoFactorAsync(TwoFactorValidationRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.ValidateTwoFactorAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> VerifyEmailAsync(string token, string secret, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.VerifyEmailAsync(token, secret, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}