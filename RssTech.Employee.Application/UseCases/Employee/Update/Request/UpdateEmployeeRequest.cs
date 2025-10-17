using MediatR;
using RssTech.Employee.Application.UseCases.Employee.Update.Response;
using RssTech.Employee.Common.Contracts;
using System.ComponentModel.DataAnnotations;

namespace RssTech.Employee.Application.UseCases.Employee.Update.Request;

public class UpdateEmployeeRequest : IRequest<Result<UpdateEmployeeResponse>>
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "First name field can not be empty")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name field can not be empty")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Password field can not be empty")]
    [RegularExpression("^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!#$%&? \"]).*$", ErrorMessage = "Password must have capital letters, digits and special characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Phone number field can not be empty")]
    public string PhoneNumber1 { get; set; }

    public string? PhoneNumber2 { get; set; }
}