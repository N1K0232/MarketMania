using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MarketMania.Shared.Models.Requests;

namespace MarketMania.BusinessLayer.Validations;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(r => r.AccessToken)
            .NotEmpty()
            .WithMessage("Access token is required");

        RuleFor(r => r.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}