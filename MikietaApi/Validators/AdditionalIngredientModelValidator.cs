using FluentValidation;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class AdditionalIngredientModelValidator : AbstractValidator<AdditionalIngredientModel>
{
    public AdditionalIngredientModelValidator()
    {
        RuleFor(x => x.IngredientId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}