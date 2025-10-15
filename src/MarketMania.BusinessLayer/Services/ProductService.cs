using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using AutoMapper;
using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class ProductService(IApplicationDbContext applicationDbContext, IProductCodeGenerator productCodeGenerator, TimeProvider timeProvider, IMapper mapper) : IProductService
{
    public async Task<Result> ConfirmAsync(Guid id, CancellationToken cancellationToken)
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

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await applicationDbContext.GetAsync<Entities.Product>(id, cancellationToken);
        if (product is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
        }

        await applicationDbContext.DeleteAsync(product, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        return Result.Ok();
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
        var dbProduct = mapper.Map<Entities.Product>(request);
        dbProduct.TotalPrice = CalculateTotalPrice(request);

        dbProduct.Barcode = await GenerateBarCodeAsync(cancellationToken);
        dbProduct.Code = await GenerateCodeAsync(cancellationToken);

        dbProduct.SerialNumber = await GenerateSerialNumberAsync(cancellationToken);
        dbProduct.SKUCode = await GenerateSKUCodeAsync(cancellationToken);

        await applicationDbContext.InsertAsync(dbProduct, cancellationToken);
        await applicationDbContext.SaveAsync(cancellationToken);

        var savedProduct = mapper.Map<Product>(dbProduct);
        return savedProduct;
    }

    public async Task<Result> UpdateAsync(Guid id, SaveProductRequest request, CancellationToken cancellationToken)
    {
        var dbProduct = await applicationDbContext.GetData<Entities.Product>(true).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (dbProduct is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Product not found", $"No product found with id {id}");
        }

        mapper.Map(request, dbProduct);
        dbProduct.TotalPrice = CalculateTotalPrice(request);

        await applicationDbContext.SaveAsync(cancellationToken);
        return Result.Ok();
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

    private async Task<string> GenerateCodeAsync(CancellationToken cancellationToken)
    {
        string code;
        var query = applicationDbContext.GetData<Entities.Product>();

        do
        {
            code = await productCodeGenerator.GenerateCodeAsync(cancellationToken);
        }
        while (await query.AnyAsync(p => p.Code == code, cancellationToken));

        return code;
    }

    private async Task<string> GenerateBarCodeAsync(CancellationToken cancellationToken)
    {
        string barcode;
        var query = applicationDbContext.GetData<Entities.Product>();

        do
        {
            barcode = await productCodeGenerator.GenerateBarcodeAsync(cancellationToken);
        } while (await query.AnyAsync(p => p.Barcode == barcode, cancellationToken));

        return barcode;
    }

    private async Task<string> GenerateSerialNumberAsync(CancellationToken cancellationToken)
    {
        string serialNumber;
        var query = applicationDbContext.GetData<Entities.Product>();

        do
        {
            serialNumber = await productCodeGenerator.GenerateSerialNumberAsync(cancellationToken);
        }
        while (await query.AnyAsync(p => p.SerialNumber == serialNumber, cancellationToken));

        return serialNumber;
    }

    private async Task<string> GenerateSKUCodeAsync(CancellationToken cancellationToken)
    {
        string skuCode;
        var query = applicationDbContext.GetData<Entities.Product>();

        do
        {
            skuCode = await productCodeGenerator.GenerateSKUAsync(cancellationToken);
        }
        while (await query.AnyAsync(p => p.SKUCode == skuCode, cancellationToken));

        return skuCode;
    }
}