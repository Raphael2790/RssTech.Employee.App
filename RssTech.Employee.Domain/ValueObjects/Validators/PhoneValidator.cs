using FluentValidation;

namespace RssTech.Employee.Domain.ValueObjects.Validators;

public sealed class PhoneValidator : AbstractValidator<Phone>
{
    public static readonly PhoneValidator Instance = new();

    public PhoneValidator()
    {
        RuleFor(phone => phone.Number)
            .NotEmpty()
            .WithMessage("Phone number cannot be empty")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Invalid phone number format");
    }
}
