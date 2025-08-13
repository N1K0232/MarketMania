using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class SaveSpecificationRequestValidator : AbstractValidator<SaveSpecificationRequest>
{
    public SaveSpecificationRequestValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(256)
            .WithMessage("Maximum length is 256 characters");

        RuleFor(s => s.Description)
            .NotEmpty()
            .WithMessage("Description is required");
    }
}