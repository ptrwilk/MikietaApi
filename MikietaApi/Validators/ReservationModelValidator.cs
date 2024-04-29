using FluentValidation;
using MikietaApi.Helpers;
using MikietaApi.Models;

namespace MikietaApi.Validators;

public class ReservationModelValidator : AbstractValidator<ReservationModel>
{
    public ReservationModelValidator()
    {
        RuleFor(x => x).Must(x => x.ReservationDate > DateTime.Now)
            .WithMessage($"{nameof(ReservationModel.ReservationDate)} must be greater than {DateTime.Now}");
        RuleFor(x => x.NumberOfPeople).Must(x => x > 0)
            .WithMessage($"{nameof(ReservationModel.NumberOfPeople)} must be greater than 0");
        RuleFor(x => x.Phone).Must(x =>
            RegexHelper.IsMatch("(^\\+?\\d{1,2}\\s?\\d{3}\\s?\\d{3}\\s?\\d{3}$)|(^\\d{3}\\s?\\d{3}\\s?\\d{3}$)", x))
            .WithMessage($"{nameof(ReservationModel.Phone)} has incorrect format");
        RuleFor(x => x.Email).Must(x =>
                RegexHelper.IsMatch("^[a-zA-Z0-9]+@[a-zA-Z0-9]+\\.[a-zA-Z0-9]+$", x))
            .WithMessage($"{nameof(ReservationModel.Email)} has incorrect format");
        RuleFor(x => x.Name).NotEmpty();
    }
}