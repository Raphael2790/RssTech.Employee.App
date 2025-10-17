using RssTech.Employee.Application.UseCases.Employee.Create.Request;

namespace RssTech.Employee.Application.Mappings;

public static class EmployeeMapper
{
    public static Domain.Entities.Employee FromRequest(CreateEmployeeRequest createEmployee)
        => new(
            createEmployee.FirstName,
            createEmployee.LastName,
            new Domain.ValueObjects.Email(createEmployee.Email),
            createEmployee.Password,
            new Domain.ValueObjects.EmployeeDocument(createEmployee.DocumentNumber),
            new List<Domain.ValueObjects.Phone>
            {
                new(createEmployee.PhoneNumber1),
                !string.IsNullOrWhiteSpace(createEmployee.PhoneNumber2) ? new(createEmployee.PhoneNumber2) : null
            }.Where(phone => phone is not null).ToList()!,
            createEmployee.DateOfBirth,
            createEmployee.Role,
            string.IsNullOrWhiteSpace(createEmployee.ManagerName) ? null : Guid.Parse(createEmployee.ManagerName)
        );
}
