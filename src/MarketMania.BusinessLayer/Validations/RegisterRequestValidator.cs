using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.FirstName)
            .MaximumLength(256)
            .WithMessage("First name max length is 256")
            .NotEmpty()
            .WithMessage("First name is required");

        RuleFor(r => r.LastName)
            .MaximumLength(256)
            .WithMessage("Last name max length is 256")
            .NotEmpty()
            .WithMessage("Last name is required");

        RuleFor(r => r.Email)
            .EmailAddress()
            .NotEmpty()
            .WithMessage("Email is required");

        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}