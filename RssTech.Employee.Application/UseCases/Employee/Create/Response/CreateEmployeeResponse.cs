using RssTech.Employee.Domain.Enums;

namespace RssTech.Employee.Application.UseCases.Employee.Create.Response;

public record struct CreateEmployeeResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public EmployeeRole Role { get; set; }
}
