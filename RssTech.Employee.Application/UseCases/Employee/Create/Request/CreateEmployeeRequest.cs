using MediatR;
using RssTech.Employee.Application.UseCases.Employee.Create.Response;
using RssTech.Employee.Common.Contracts;
using RssTech.Employee.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RssTech.Employee.Application.UseCases.Employee.Create.Request;

public record struct CreateEmployeeRequest : IRequest<Result<CreateEmployeeResponse>>
{
    [Required(ErrorMessage = "First name field can not be empty")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name field can not be empty")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email field can not be empty")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password field can not be empty")]
    [RegularExpression("^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!#$%&? \"]).*$", ErrorMessage = "Password must have capital letters, digits and special characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Document number field can not be empty")]
    public string DocumentNumber { get; set; }

    [Required(ErrorMessage = "Phone number field can not be empty")]
    public string PhoneNumber1 { get; set; }

    public string PhoneNumber2 { get; set; }

    public DateTime DateOfBirth { get; set; }

    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;

    public string ManagerName { get; set; } = string.Empty;

    public CreateEmployeeRequest()
    {
    }
}
