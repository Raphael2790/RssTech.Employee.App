using MediatR;
using RssTech.Employee.Application.UseCases.Employee.Create.Response;
using RssTech.Employee.Common.Contracts;
using RssTech.Employee.Domain.Enums;

namespace RssTech.Employee.Application.UseCases.Employee.Create.Request;

public record struct CreateEmployeeRequest : IRequest<Result<CreateEmployeeResponse>>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string DocumentNumber { get; set; }

    public string PhoneNumber1 { get; set; }

    public string PhoneNumber2 { get; set; }

    public DateTime DateOfBirth { get; set; }

    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;

    public string ManagerName { get; set; }

    public CreateEmployeeRequest()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        DocumentNumber = string.Empty;
        PhoneNumber1 = string.Empty;
        PhoneNumber2 = string.Empty;
        ManagerName = string.Empty;
    }
}
