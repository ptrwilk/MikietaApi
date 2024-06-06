using FluentValidation;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class DeliveryModelValidator: AbstractValidator<DeliveryModel>
{
    public DeliveryModelValidator()
    {
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.HomeNumber).NotEmpty();
    }
}