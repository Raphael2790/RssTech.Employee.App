namespace RssTech.Employee.Application.UseCases.Employee.Create.Response;

public record struct CreateEmployeeResponse
{
    public Guid Id { get; set; }
    public string CreatedAt { get; set; }
}
