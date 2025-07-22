using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class SavePromotionRequestValidator : AbstractValidator<SavePromotionRequest>
{
    public SavePromotionRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(255)
            .WithMessage("Name can be only 255 characters long");

        RuleFor(p => p.DiscountPercentage)
            .PrecisionScale(5, 2, true)
            .WithMessage("Please insert a valid percentage");

        RuleFor(p => p.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("You can't insert a past date");

        RuleFor(p => p.EndDate)
            .GreaterThan(p => p.StartDate)
            .WithMessage("You can't insert a past date or a date less than the start date");
    }
}