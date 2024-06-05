using FluentValidation;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class RemovedIngredientModelValidator : AbstractValidator<RemovedIngredientModel>
{
    public RemovedIngredientModelValidator()
    {
        RuleFor(x => x.IngredientId).NotEmpty();
    }
}