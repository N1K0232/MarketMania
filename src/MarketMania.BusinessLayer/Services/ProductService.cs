using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using AutoMapper;
using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Notifications;
using MarketMania.Shared.Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OperationResults;
using SimpleTransit;
using TinyHelpers.Extensions;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class ProductService(IApplicationDbContext applicationDbContext, IServiceProvider serviceProvider, INotificationPublisher notificationPublisher, IMapper mapper) : IProductService
{
    public async Task<Result> ConfirmAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var product = await applicationDbContext.GetAsync<Entities.Product>(id, cancellationToken);
            if (product is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
            }

            product.IsPublished = true;
            product.PublishedAt = DateTime.UtcNow;

            await applicationDbContext.SaveAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to update the product", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to update the product", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var product = await applicationDbContext.GetAsync<Entities.Product>(id, cancellationToken);
            if (product is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
            }

            await applicationDbContext.DeleteAsync(product, cancellationToken);
            await applicationDbContext.SaveAsync(cancellationToken);

            await notificationPublisher.NotifyAsync(new ProductDeleted(id), cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to delete the product", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to delete the product", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", ex.Message);
        }
    }

    public async Task<Result<Product>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbProduct = await applicationDbContext.GetData<Entities.Product>()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .Include(p => p.Specifications)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (dbProduct is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
        }

        if (!dbProduct.IsPublished)
        {
            return Result.Fail(FailureReasons.Forbidden, "You're not allowed to see this product", "You're not allowed to see this product");
        }

        var product = mapper.Map<Product>(dbProduct);
        return product;
    }

    public async Task<Result<PaginatedList<Product>>> GetListAsync(string? searchText, int pageIndex, int itemsPerPage, string orderBy, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.GetData<Entities.Product>()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Include(p => p.Images)
            .Include(p => p.Ratings)
            .Include(p => p.Specifications)
            .WhereIf(searchText.HasValue(), p => p.Name.Contains(searchText!) || p.Brand.Name.Contains(searchText!) || p.Category.Name.Contains(searchText!))
            .Where(p => p.IsPublished);

        var totalCount = await query.CountAsync(cancellationToken);

        try
        {
            query = query.OrderBy(orderBy);
        }
        catch (ParseException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to order", ex.Message);
        }

        var dbProducts = await query.Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1).ToListAsync(cancellationToken);
        var products = mapper.Map<IEnumerable<Product>>(dbProducts).Take(itemsPerPage);

        var list = new PaginatedList<Product>(products, totalCount, dbProducts.Count > itemsPerPage);
        return list;
    }

    public async Task<Result<Product>> InsertAsync(SaveProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dbProduct = mapper.Map<Entities.Product>(request);
            dbProduct.TotalPrice = CalculateTotalPrice(request);

            await applicationDbContext.InsertAsync(dbProduct, cancellationToken);
            await ProtectAsync(dbProduct, cancellationToken);

            await applicationDbContext.SaveAsync(cancellationToken);
            await notificationPublisher.NotifyAsync(new ProductCreated(dbProduct.Id), cancellationToken);

            var createdProduct = mapper.Map<Product>(dbProduct);
            return createdProduct;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to create the product", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to create the product", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(Guid id, SaveProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var dbProduct = await applicationDbContext.GetData<Entities.Product>(true).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (dbProduct is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
            }

            mapper.Map(request, dbProduct);
            dbProduct.TotalPrice = CalculateTotalPrice(request);

            await ProtectAsync(dbProduct, cancellationToken);
            await applicationDbContext.SaveAsync(cancellationToken);

            await notificationPublisher.NotifyAsync(new ProductUpdated(id), cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to update the product", ex.Message);
        }
        catch (SqlException ex)
        {
            if (ex.ErrorCode is 2627)
            {
                return Result.Fail(FailureReasons.ClientError, "Unable to update the product", ex.Message);
            }

            return Result.Fail(FailureReasons.DatabaseError, "Database error", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No product found", ex.Message);
        }
    }

    private static decimal CalculateTotalPrice(SaveProductRequest request)
    {
        var price = request.Price;
        var discountPercentage = request.DiscountPercentage.GetValueOrDefault();

        var taxes = request.Taxes.GetValueOrDefault();
        var shippingCost = request.ShippingCost.GetValueOrDefault();

        var discountAmount = price * Convert.ToDecimal((discountPercentage / 100));
        return price - discountAmount + Convert.ToDecimal(taxes) + shippingCost;
    }

    private async Task ProtectAsync(Entities.Product product, CancellationToken cancellationToken)
    {
        var barcodeGenerator = serviceProvider.GetRequiredService<IBarcodeGenerator>();
        await barcodeGenerator.GenerateAsync(product, cancellationToken);

        var codeGenerator = serviceProvider.GetRequiredService<ICodeGenerator>();
        await codeGenerator.GenerateAsync(product, cancellationToken);

        var skuCodeGenerator = serviceProvider.GetRequiredService<ISKUCodeGenerator>();
        await skuCodeGenerator.GenerateAsync(product, cancellationToken);

        var serialNumberGenerator = serviceProvider.GetRequiredService<ISerialNumberGenerator>();
        await serialNumberGenerator.GenerateAsync(product, cancellationToken);
    }
}