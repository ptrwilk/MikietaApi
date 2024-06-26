﻿using FluentValidation;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class ProductQuantityModelValidator: AbstractValidator<ProductQuantityModel>
{
    public ProductQuantityModelValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}