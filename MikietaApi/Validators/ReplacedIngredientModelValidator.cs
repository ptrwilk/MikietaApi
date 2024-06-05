using FluentValidation;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class ReplacedIngredientModelValidator : AbstractValidator<ReplacedIngredientModel>
{
    public ReplacedIngredientModelValidator()
    {
        RuleFor(x => x.FromIngredientId).NotEmpty();
        RuleFor(x => x.ToIngredientId).NotEmpty();
    }
}