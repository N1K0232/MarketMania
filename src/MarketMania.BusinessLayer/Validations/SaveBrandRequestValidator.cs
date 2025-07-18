using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class SaveBrandRequestValidator : AbstractValidator<SaveBrandRequest>
{
    public SaveBrandRequestValidator()
    {
        RuleFor(b => b.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(b => b.City)
            .NotEmpty()
            .WithMessage("City is required");
    }
}