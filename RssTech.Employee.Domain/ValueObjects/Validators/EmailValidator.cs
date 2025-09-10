using FluentValidation;

namespace RssTech.Employee.Domain.ValueObjects.Validators;

public sealed class EmailValidator : AbstractValidator<Email>
{
    public static readonly EmailValidator Instance = new();

    public EmailValidator()
    {
        RuleFor(email => email.Address)
            .NotEmpty()
            .WithMessage("Email address cannot be empty.")
            .EmailAddress()
            .WithMessage("Invalid email address format.");
    }
}
