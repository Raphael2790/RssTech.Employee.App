namespace RssTech.Employee.Application.UseCases.Employee.Get.Request;

public record struct GetEmployeeRequest
{
    public Guid Id { get; set; }
}
