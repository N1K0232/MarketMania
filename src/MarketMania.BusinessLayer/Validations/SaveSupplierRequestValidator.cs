using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class SaveSupplierRequestValidator : AbstractValidator<SaveSupplierRequest>
{
    public SaveSupplierRequestValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(s => s.City)
            .NotEmpty()
            .WithMessage("City is required");
    }
}