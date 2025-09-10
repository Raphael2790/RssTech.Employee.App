namespace RssTech.Employee.Application.UseCases.Employee.Update.Response;

public record struct UpdateEmployeeResponse
{
    public Guid Id { get; set; }
    public string UpdatedAt { get; set; }
}
