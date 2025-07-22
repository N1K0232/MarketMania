using FluentValidation;
using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Entities;
using MarketMania.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace MarketMania.BusinessLayer.Validations;

public class SaveProductRequestValidator : AbstractValidator<SaveProductRequest>
{
    private readonly IApplicationDbContext applicationDbContext;

    public SaveProductRequestValidator(IApplicationDbContext applicationDbContext)
    {
        this.applicationDbContext = applicationDbContext;

        RuleFor(p => p.BrandId)
            .NotEmpty()
            .WithMessage("Brand is required")
            .MustAsync(BrandMustExistsAsync)
            .WithMessage("Brand doesn't exists");

        RuleFor(p => p.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required")
            .MustAsync(CategoryMustExistsAsync)
            .WithMessage("Category doesn't exists");

        RuleFor(p => p.SupplierId)
            .NotEmpty()
            .WithMessage("Supplier is required")
            .MustAsync(SupplierMustExistsAsync)
            .WithMessage("Supplier doesn't exists");

        RuleFor(p => p.PromotionId)
            .MustAsync(PromotionMustExistsAsync)
            .WithMessage("Promotion doesn't exists");


        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(255)
            .WithMessage("Name is maximum 255 characters");

        RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(4000)
            .WithMessage("Description is maximum 4000 characters");

        RuleFor(p => p.Quantity)
            .GreaterThan(0)
            .WithMessage("Cannot create a product with 0 or negative quantity");

        RuleFor(p => p.Price)
            .GreaterThan(0)
            .WithMessage("Cannot create a product with 0 or negative total price")
            .PrecisionScale(18, 2, true)
            .WithMessage("Please insert a valid price");

        RuleFor(p => p.DiscountPercentage)
            .GreaterThan(0)
            .WithMessage("Cannot create a product with 0 or negative discount percentage");

        RuleFor(p => p.Taxes)
            .GreaterThan(0)
            .WithMessage("Cannot create a product with 0 or negative taxes");

        RuleFor(p => p.ShippingCost)
            .GreaterThan(0)
            .WithMessage("Cannot create a product with 0 or negative shipping cost")
            .PrecisionScale(5, 2, true)
            .WithMessage("Please insert a valid value for the shipping cost");

        RuleFor(p => p.Tags)
            .Must(t => t.Length > 0)
            .WithMessage("At least one tag is required");

        RuleFor(p => p.SeoTitle)
            .MaximumLength(255)
            .WithMessage("Maximum length for the SEO title is 255 characters");

        RuleFor(p => p.SeoDescription)
            .MaximumLength(4000)
            .WithMessage("Maximum length for the SEO description is 4000 characters");

        RuleFor(p => p.SKU)
            .GreaterThan(0)
            .WithMessage("Cannot insert a product with 0 or negative SKU");

        RuleFor(p => p.Weight)
            .GreaterThan(0)
            .WithMessage("Cannot insert a product with 0 or negative weight");

        RuleFor(p => p.Width)
            .GreaterThan(0)
            .WithMessage("Cannot insert a product with 0 or negative width");

        RuleFor(p => p.Height)
            .GreaterThan(0)
            .WithMessage("Cannot insert a product with 0 or negative height");

        RuleFor(p => p.Length)
            .GreaterThan(0)
            .WithMessage("Cannot insert a product with 0 or negative length");

        RuleFor(p => p.WarehouseLocation)
            .NotEmpty()
            .WithMessage("Warehouse location is required")
            .MaximumLength(100)
            .WithMessage("Warehouse should be only 100 characters long");

        RuleFor(p => p.MinStockAlert)
            .GreaterThan(0)
            .WithMessage("Cannot insert a product with 0 or negative alert for minimum stock");

        RuleFor(p => p.RestockDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Cannot insert a product with past restock date");
    }

    private async Task<bool> BrandMustExistsAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.GetData<Brand>();
        return await query.AnyAsync(b => b.Id == brandId, cancellationToken);
    }

    private async Task<bool> CategoryMustExistsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.GetData<Category>();
        return await query.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }

    private async Task<bool> SupplierMustExistsAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.GetData<Supplier>();
        return await query.AnyAsync(s => s.Id == supplierId, cancellationToken);
    }

    private async Task<bool> PromotionMustExistsAsync(Guid? promotionId, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.GetData<Promotion>();
        return await query.AnyAsync(p => p.Id == promotionId, cancellationToken);
    }
}