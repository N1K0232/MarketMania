using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class TwoFactorValidationRequestValidator : AbstractValidator<TwoFactorValidationRequest>
{
    public TwoFactorValidationRequestValidator()
    {
        RuleFor(t => t.Token)
            .NotEmpty()
            .WithMessage("Token is required");

        RuleFor(t => t.Code)
            .NotEmpty()
            .WithMessage("Code is required");
    }
}