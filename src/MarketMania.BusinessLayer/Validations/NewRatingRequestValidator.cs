using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class NewRatingRequestValidator : AbstractValidator<NewRatingRequest>
{
    public NewRatingRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(255)
            .WithMessage("Title can be only 255 characters long");

        RuleFor(r => r.Text)
            .NotEmpty()
            .WithMessage("Text is required");

        RuleFor(r => r.Score)
            .InclusiveBetween(1, 5)
            .WithMessage("Please insert a score between 1 and 5");
    }
}