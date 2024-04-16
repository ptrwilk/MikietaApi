using FluentValidation;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class OrderModelValidator : AbstractValidator<OrderModel>
{
    public OrderModelValidator()
    {
        RuleFor(x => x.ProductIds).NotEmpty();
        RuleFor(x => x).Must(x => x.DeliveryTiming != null || x.DeliveryRightAway == true).WithMessage(
            $"Either {nameof(OrderModel.DeliveryTiming)} or {nameof(OrderModel.DeliveryRightAway)} must have a value.");
        RuleFor(x => x.DeliveryMethod).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x).Must(x =>
            x.ProcessingPersonalData == null || (x.ProcessingPersonalData != null && (x.ProcessingPersonalData.Email.HasValue || x.ProcessingPersonalData.Sms.HasValue)))
            .WithMessage($"Either {nameof(ProcessingPersonalData.Email)} or {nameof(ProcessingPersonalData.Sms)} must be true.");
    }
}