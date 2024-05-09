using FluentValidation;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class OrderModelValidator : AbstractValidator<OrderModel>
{
    public OrderModelValidator()
    {
        RuleFor(x => x.ProductQuantities).NotEmpty();
        RuleForEach(x => x.ProductQuantities).SetValidator(new ProductQuantityModelValidator());
        RuleFor(x => x).Must(x => x.DeliveryTiming != null || x.DeliveryRightAway == true).WithMessage(
            $"Either {nameof(OrderModel.DeliveryTiming)} or {nameof(OrderModel.DeliveryRightAway)} must have a value.");
        RuleFor(x => x.DeliveryMethod).NotNull();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x).Must(x =>
                x.ProcessingPersonalData == null || (x.ProcessingPersonalData != null &&
                                                     (x.ProcessingPersonalData.Email.HasValue ||
                                                      x.ProcessingPersonalData.Sms.HasValue)))
            .WithMessage(
                $"Either {nameof(ProcessingPersonalData.Email)} or {nameof(ProcessingPersonalData.Sms)} must be true.");
        RuleFor(x => x).Must(x =>
            x.DeliveryMethod != DeliveryMethodType.Delivery || (x.DeliveryMethod == DeliveryMethodType.Delivery &&
                                                                !string.IsNullOrEmpty(x.Street) &&
                                                                !string.IsNullOrEmpty(x.HomeNumber) &&
                                                                !string.IsNullOrEmpty(x.City) &&
                                                                !string.IsNullOrEmpty(x.FlatNumber) &&
                                                                !string.IsNullOrEmpty(x.Floor))
        ).WithMessage("Street, Home Number, City, Flat Number and Floor must be provided when Delivery Method is Delivery");
    }
}