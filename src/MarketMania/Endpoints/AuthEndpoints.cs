using System.Net.Mime;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace MarketMania.Endpoints;

public class AuthEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var authApiGroup = endpoints.MapGroup("/api/auth").WithTags("Auth");

        authApiGroup.MapGet("qrcode", GetQRCodeAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Image.Png)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("qrcode");

        authApiGroup.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .WithValidation<LoginRequest>()
            .Produces<AuthResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("login");

        authApiGroup.MapPost("logout", LogoutAsync)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("logout");

        authApiGroup.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidation<RegisterRequest>()
            .Produces(StatusCodes.Status201Created)
            .WithName("register");

        authApiGroup.MapPost("validate2fa", ValidateTwoFactorAsync)
            .AllowAnonymous()
            .WithValidation<TwoFactorValidationRequest>()
            .Produces<AuthResponse>()
            .WithName("validate2fa");

        authApiGroup.MapGet("verifyemail", VerifyEmailAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("verifyemail");
    }

    private static async Task<IResult> GetQRCodeAsync(string token, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.GetQRCodeAsync(token, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> LoginAsync(LoginRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.LoginAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> LogoutAsync(IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.LogoutAsync(httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> RegisterAsync(RegisterRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.RegisterAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, StatusCodes.Status201Created);
        return response;
    }

    private static async Task<IResult> ValidateTwoFactorAsync(TwoFactorValidationRequest request, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.ValidateTwoFactorAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> VerifyEmailAsync(string secret, string token, IIdentityService identityService, HttpContext httpContext)
    {
        var result = await identityService.VerifyEmailAsync(secret, token, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}