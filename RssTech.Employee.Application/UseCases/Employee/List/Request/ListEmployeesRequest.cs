namespace RssTech.Employee.Application.UseCases.Employee.List.Request;

public record struct ListEmployeesRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
