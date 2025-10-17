using RssTech.Employee.Domain.Enums;

namespace RssTech.Employee.Application.UseCases.Employee.Get.Response;

public class GetEmployeeResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Guid? ManagerName { get; set; }
}