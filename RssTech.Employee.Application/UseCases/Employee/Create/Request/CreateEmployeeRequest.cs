using MediatR;
using RssTech.Employee.Application.UseCases.Employee.Create.Response;

namespace RssTech.Employee.Application.UseCases.Employee.Create.Request;

public record struct CreateEmployeeRequest : IRequest<CreateEmployeeResponse>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string DocumentNumber { get; set; }

    public string PhoneNumber1 { get; set; }

    public string PhoneNumber2 { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string ManagerName { get; set; }
}
