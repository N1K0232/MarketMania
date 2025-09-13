using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using MarketMania.BusinessLayer.Services;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults;
using OperationResults.AspNetCore.Http;
using TinyHelpers.AspNetCore.DataAnnotations;

namespace MarketMania.Endpoints;

public class ProductEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var productsApiGroup = endpoints.MapGroup("/api/products");

        productsApiGroup.MapPost("{id:guid}/confirm", ConfirmAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("ConfirmProduct")
            .WithOpenApi();

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

        productsApiGroup.MapDelete("{productId:guid}/images/{imageId:guid}", DeleteImageAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteProductImage")
            .WithOpenApi();

        productsApiGroup.MapGet("{productId:guid}/images/{imageId:guid}", GetImageAsync)
            .AllowAnonymous()
            .Produces<Image>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetProductImage")
            .WithOpenApi();

        productsApiGroup.MapGet("{productId:guid}/images", GetImagesAsync)
            .AllowAnonymous()
            .Produces<PaginatedList<Image>>()
            .WithName("GetProductImages")
            .WithOpenApi();

        productsApiGroup.MapGet("{productId:guid}/images/{imageId:guid}/stream", ReadStreamAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Image.Jpeg)
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Image.Png)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("ReadImageStream")
            .WithOpenApi();

        productsApiGroup.MapPost("{productId:guid}/images", UploadImageAsync)
            .RequireAuthorization("Admin")
            .DisableAntiforgery()
            .Produces<Image>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .WithName("UploadProductImage")
            .WithOpenApi();

        productsApiGroup.MapDelete("{productId:guid}/ratings/{ratingId:guid}", DeleteRatingAsync)
            .AllowAnonymous()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteProductRating")
            .WithOpenApi();

        productsApiGroup.MapGet("{productId:guid}/ratings/{ratingId:guid}", GetRatingAsync)
            .AllowAnonymous()
            .Produces<Rating>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetProductRating")
            .WithOpenApi();

        productsApiGroup.MapGet("{productId:guid}/ratings", GetRatingsAsync)
            .AllowAnonymous()
            .Produces<IEnumerable<Rating>>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetProductRatings")
            .WithOpenApi();

        productsApiGroup.MapPost("{productId:guid}/ratings", PublishRatingAsync)
            .RequireAuthorization()
            .WithValidation<NewRatingRequest>()
            .Produces<Rating>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("PublishProductRating")
            .WithOpenApi();

        productsApiGroup.MapDelete("{productId:guid}/specifications/{specificationId:guid}", DeleteSpecificationAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteProductSpecification")
            .WithOpenApi();

        productsApiGroup.MapGet("{productId:guid}/specifications/{specificationId:guid}", GetSpecificationAsync)
            .AllowAnonymous()
            .Produces<Specification>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetProductSpecification")
            .WithOpenApi();

        productsApiGroup.MapGet("{productId:guid}/specifications", GetSpecificationsAsync)
            .AllowAnonymous()
            .Produces<IEnumerable<Specification>>()
            .WithName("GetProductSpecifications")
            .WithOpenApi();

        productsApiGroup.MapPost("{productId:guid}/specifications", InsertSpecificationAsync)
            .RequireAuthorization("Admin")
            .Produces<Specification>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .WithValidation<SaveSpecificationRequest>()
            .WithName("InsertProductSpecification")
            .WithOpenApi();

        productsApiGroup.MapPut("{productId:guid}/specifications/{specificationId:guid}", UpdateSpecificationAsync)
            .RequireAuthorization("Admin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithValidation<SaveSpecificationRequest>()
            .WithName("UpdateProductSpecification")
            .WithOpenApi();
    }

    public static async Task<IResult> ConfirmAsync(Guid id, IProductService productService, HttpContext httpContext)
    {
        var result = await productService.ConfirmAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
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

    public static async Task<IResult> GetListAsync(IProductService productService, HttpContext httpContext, string name, string brand, string category, int pageIndex = 0, int itemsPerPage = 50, string orderBy = "Name, Price")
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

    public static async Task<IResult> DeleteImageAsync(Guid productId, Guid imageId, IImageService imageService, HttpContext httpContext)
    {
        var result = await imageService.DeleteAsync(productId, imageId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetImageAsync(Guid productId, Guid imageId, IImageService imageService, HttpContext httpContext)
    {
        var result = await imageService.GetAsync(productId, imageId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetImagesAsync(Guid productId, IImageService imageService, HttpContext httpContext)
    {
        var result = await imageService.GetListAsync(productId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> ReadStreamAsync(Guid productId, Guid imageId, IImageService imageService, HttpContext httpContext)
    {
        var result = await imageService.ReadAsync(productId, imageId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> UploadImageAsync(Guid productId, [Required][AllowedExtensions("*.jpg", "*.png")] IFormFile file, IImageService imageService, HttpContext httpContext)
    {
        var result = await imageService.UploadAsync(productId, file, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetProductImage", new { productId = productId, imageId = result.Content?.Id });
        return response;
    }

    public static async Task<IResult> DeleteRatingAsync(Guid productId, Guid ratingId, IRatingService ratingService, HttpContext httpContext)
    {
        var result = await ratingService.DeleteAsync(productId, ratingId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetRatingAsync(Guid productId, Guid ratingId, IRatingService ratingService, HttpContext httpContext)
    {
        var result = await ratingService.GetAsync(productId, ratingId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetRatingsAsync(Guid productId, IRatingService ratingService, HttpContext httpContext)
    {
        var result = await ratingService.GetListAsync(productId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> PublishRatingAsync(Guid productId, NewRatingRequest request, IRatingService ratingService, HttpContext httpContext)
    {
        var result = await ratingService.PublishAsync(productId, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetProductRating", new { productId, result.Content?.Id });
        return response;
    }

    public static async Task<IResult> DeleteSpecificationAsync(Guid productId, Guid specificationId, ISpecificationService specificationService, HttpContext httpContext)
    {
        var result = await specificationService.DeleteAsync(productId, specificationId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetSpecificationAsync(Guid productId, Guid specificationId, ISpecificationService specificationService, HttpContext httpContext)
    {
        var result = await specificationService.GetAsync(productId, specificationId, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> GetSpecificationsAsync(Guid productId, string name, ISpecificationService specificationService, HttpContext httpContext)
    {
        var result = await specificationService.GetListAsync(productId, name, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    public static async Task<IResult> InsertSpecificationAsync(Guid productId, SaveSpecificationRequest request, ISpecificationService specificationService, HttpContext httpContext)
    {
        var result = await specificationService.InsertAsync(productId, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetProductSpecification", new { productId = productId, specificationId = result.Content?.Id });
        return response;
    }

    public static async Task<IResult> UpdateSpecificationAsync(Guid productId, Guid specificationId, SaveSpecificationRequest request, ISpecificationService specificationService, HttpContext httpContext)
    {
        var result = await specificationService.UpdateAsync(productId, specificationId, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}