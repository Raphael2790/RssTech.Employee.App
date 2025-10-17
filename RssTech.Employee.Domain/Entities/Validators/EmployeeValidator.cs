using FluentValidation;
using RssTech.Employee.Domain.ValueObjects.Validators;

namespace RssTech.Employee.Domain.Entities.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
    public static EmployeeValidator Instance { get; } = new();

    public EmployeeValidator()
    {
        RuleFor(e => e.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(e => e.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(e => e.Email)
            .NotNull().WithMessage("Email is required.")
            .SetValidator(EmailValidator.Instance);

        RuleFor(e => e.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

        RuleFor(e => e.Document)
            .NotNull().WithMessage("Document is required.")
            .SetValidator(EmployeeDocumentValidator.Instance);

        RuleForEach(e => e.Phones)
            .SetValidator(PhoneValidator.Instance);
    }
}
