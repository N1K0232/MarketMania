using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using AutoMapper;
using MarketMania.DataAccessLayer;
using MarketMania.Shared.Models;
using MarketMania.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = MarketMania.DataAccessLayer.Entities;

namespace MarketMania.BusinessLayer.Services;

public class ProductService(IApplicationDbContext applicationDbContext, IMapper mapper) : IProductService
{
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
        var dbProduct = await applicationDbContext.GetAsync<Entities.Product>(id, cancellationToken);
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

    public async Task<Result<PaginatedList<Product>>> GetListAsync(string name, string brand, string category, int pageIndex, int itemsPerPage, string orderBy, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.GetData<Entities.Product>()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .WhereIf(name.HasValue(), p => p.Name.Contains(name))
            .WhereIf(brand.HasValue(), p => p.Brand.Name.Contains(brand))
            .WhereIf(category.HasValue(), p => p.Category.Name.Contains(category))
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

        dbProduct.SerialNumber = await GenerateSerialNumberAsync(cancellationToken);
        dbProduct.Barcode = await GenerateBarCodeAsync(cancellationToken);

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

    private async Task<string> GenerateBarCodeAsync(CancellationToken cancellationToken)
    {
        string barcode;
        bool exists;

        do
        {
            barcode = GenerateBarcode();
            exists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.Barcode == barcode, cancellationToken);
        } while (exists);

        return barcode;

        static string GenerateBarcode()
        {
            var random = new Random();
            var code = string.Empty;

            for (var i = 0; i < 12; i++)
            {
                code += random.Next(0, 10);
            }

            var sum = 0;

            for (var i = 0; i < 12; i++)
            {
                var digit = int.Parse(code[i].ToString());
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            var checksum = (10 - (sum % 10)) % 10;
            return code + checksum.ToString();
        }
    }

    private async Task<string> GenerateSerialNumberAsync(CancellationToken cancellationToken)
    {
        string serialNumber;
        bool exists;

        do
        {
            serialNumber = GenerateSerialNumber();
            exists = await applicationDbContext.GetData<Entities.Product>().AnyAsync(p => p.SerialNumber == serialNumber, cancellationToken);
        }
        while (exists);

        return serialNumber;

        static string GenerateSerialNumber()
            => $"SN-{Guid.CreateVersion7().ToString("N")[..12].ToUpper()}";
    }
}