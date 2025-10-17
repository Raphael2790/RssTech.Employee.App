namespace RssTech.Employee.Domain.ValueObjects;

public record class EmployeeDocument
{
    public string? DocumentNumber { get; init; }

    public EmployeeDocument(string? documentNumber)
    {
        DocumentNumber = documentNumber;
    }
}
