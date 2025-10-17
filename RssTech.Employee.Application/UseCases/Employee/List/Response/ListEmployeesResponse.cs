using RssTech.Employee.Domain.Enums;

namespace RssTech.Employee.Application.UseCases.Employee.List.Response;

public class ListEmployeesResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
}