using RssTech.Employee.Domain.Entities.Validators;
using RssTech.Employee.Domain.Enums;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Domain.Entities;

public sealed class Employee : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string Password { get; private set; }
    public EmployeeDocument Document { get; private set; }
    public List<Phone> Phones { get; private set; } = [];
    public DateTime DateOfBirth { get; private set; }
    public EmployeeRole Role { get; private set; }

    public Guid? ManagerName { get; private set; }


    public Employee(
        string firstName,
        string lastName,
        Email email,
        string password,
        EmployeeDocument document,
        List<Phone> phones,
        DateTime dateOfBirth,
        EmployeeRole role = EmployeeRole.Employee,
        Guid? managerName = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        Document = document;
        Phones = phones;
        DateOfBirth = dateOfBirth;
        Role = role;
        ManagerName = managerName;

        Validate();
    }

    public bool IsAdult()
        => DateTime.Now.Year - DateOfBirth.Year >= 18;

    protected override void Validate()
    {
        var validationResult = EmployeeValidator.Instance.Validate(this);

        if (!validationResult.IsValid)
        {
            AddNotifications(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        if (!IsAdult())
        {
            AddNotification("Employee must be at least 18 years old.");
        }
    }
}
