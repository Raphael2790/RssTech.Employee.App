using RssTech.Employee.Domain.Enums;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Domain.UnitTests.Common;

public abstract class TestBase
{
    protected static Email CreateValidEmail(string address = "test@example.com")
        => new(address);

    protected static Phone CreateValidPhone(string number = "11999999999")
        => new(number);

    protected static EmployeeDocument CreateValidDocument(string documentNumber = "123.456.789-01")
        => new(documentNumber);

    protected static List<Phone> CreateValidPhoneList()
        => [CreateValidPhone()];

    protected static DateTime CreateValidBirthDate(int yearsOld = 25)
        => DateTime.Now.AddYears(-yearsOld);

    protected static RssTech.Employee.Domain.Entities.Employee CreateValidEmployee(
        string firstName = "John",
        string lastName = "Doe",
        Email? email = null,
        string password = "Password123!",
        EmployeeDocument? document = null,
        List<Phone>? phones = null,
        DateTime? dateOfBirth = null,
        EmployeeRole role = EmployeeRole.Employee,
        Guid? managerId = null)
    {
        return new RssTech.Employee.Domain.Entities.Employee(
            firstName,
            lastName,
            email ?? CreateValidEmail(),
            password,
            document ?? CreateValidDocument(),
            phones ?? CreateValidPhoneList(),
            dateOfBirth ?? CreateValidBirthDate(),
            role,
            managerId);
    }
}