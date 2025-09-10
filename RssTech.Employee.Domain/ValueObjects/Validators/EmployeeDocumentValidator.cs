using FluentValidation;

namespace RssTech.Employee.Domain.ValueObjects.Validators;

public sealed class EmployeeDocumentValidator : AbstractValidator<EmployeeDocument>
{
    public static readonly EmployeeDocumentValidator Instance = new();

    public EmployeeDocumentValidator()
    {
        RuleFor(doc => doc.DocumentNumber)
            .NotEmpty().WithMessage("Document number cannot be empty.")
            .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")
            .WithMessage("Document number must be in the format XXX.XXX.XXX-XX.");
    }
}
