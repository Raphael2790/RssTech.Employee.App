using RssTech.Employee.Domain.Entities;
using RssTech.Employee.Domain.Enums;
using RssTech.Employee.Domain.UnitTests.Common;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Domain.UnitTests.Entities;

public class EmployeeTests : TestBase
{
    [Fact]
    public void Constructor_WhenValidParameters_ThenCreatesEmployee()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = CreateValidEmail();
        var password = "Password123!";
        var document = CreateValidDocument();
        var phones = CreateValidPhoneList();
        var dateOfBirth = CreateValidBirthDate();
        var role = EmployeeRole.Employee;

        // Act
        var employee = new RssTech.Employee.Domain.Entities.Employee(firstName, lastName, email, password, document, phones, dateOfBirth, role);

        // Assert
        Assert.Equal(firstName, employee.FirstName);
        Assert.Equal(lastName, employee.LastName);
        Assert.Equal(email, employee.Email);
        Assert.Equal(password, employee.Password);
        Assert.Equal(document, employee.Document);
        Assert.Equal(phones, employee.Phones);
        Assert.Equal(dateOfBirth, employee.DateOfBirth);
        Assert.Equal(role, employee.Role);
        Assert.Null(employee.ManagerName);
        Assert.NotEqual(Guid.Empty, employee.Id);
        Assert.True(employee.CreatedAt <= DateTime.UtcNow);
        Assert.True(employee.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WhenValidParametersWithManager_ThenCreatesEmployeeWithManager()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var employee = CreateValidEmployee(managerId: managerId);

        // Act & Assert
        Assert.Equal(managerId, employee.ManagerName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WhenInvalidFirstName_ThenIsNotValid(string? firstName)
    {
        // Arrange & Act
        var employee = CreateValidEmployee(firstName: firstName!);

        // Assert
        Assert.False(employee.IsValid);
        Assert.Contains(employee.Notifications, n => n.Contains("First name is required"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WhenInvalidLastName_ThenIsNotValid(string? lastName)
    {
        // Arrange & Act
        var employee = CreateValidEmployee(lastName: lastName!);

        // Assert
        Assert.False(employee.IsValid);
        Assert.Contains(employee.Notifications, n => n.Contains("Last name is required"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WhenInvalidPassword_ThenIsNotValid(string? password)
    {
        // Arrange & Act
        var employee = CreateValidEmployee(password: password!);

        // Assert
        Assert.False(employee.IsValid);
        Assert.Contains(employee.Notifications, n => n.Contains("Password is required"));
    }

    [Fact]
    public void Constructor_WhenUnderageEmployee_ThenIsNotValid()
    {
        // Arrange
        var underage = DateTime.Now.AddYears(-17);

        // Act
        var employee = CreateValidEmployee(dateOfBirth: underage);

        // Assert
        Assert.False(employee.IsValid);
        Assert.Contains(employee.Notifications, n => n.Contains("must be at least 18 years old"));
    }

    [Theory]
    [InlineData(18)]
    [InlineData(25)]
    [InlineData(65)]
    public void IsAdult_WhenEmployeeIsOfAge_ThenReturnsTrue(int age)
    {
        // Arrange
        var birthDate = DateTime.Now.AddYears(-age);
        var employee = CreateValidEmployee(dateOfBirth: birthDate);

        // Act
        var result = employee.IsAdult();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(17)]
    [InlineData(10)]
    [InlineData(5)]
    public void IsAdult_WhenEmployeeIsUnderage_ThenReturnsFalse(int age)
    {
        // Arrange
        var birthDate = DateTime.Now.AddYears(-age);
        var employee = CreateValidEmployee(dateOfBirth: birthDate);

        // Act
        var result = employee.IsAdult();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Constructor_WhenValidEmployee_ThenIsValid()
    {
        // Arrange & Act
        var employee = CreateValidEmployee();

        // Assert
        if (!employee.IsValid)
        {
            var notifications = string.Join(", ", employee.Notifications);
            Assert.Fail($"Employee should be valid but has notifications: {notifications}");
        }
        Assert.True(employee.IsValid);
        Assert.Empty(employee.Notifications);
    }

    [Theory]
    [InlineData(EmployeeRole.Employee)]
    [InlineData(EmployeeRole.Manager)]
    [InlineData(EmployeeRole.Director)]
    [InlineData(EmployeeRole.Administrator)]
    public void Constructor_WhenValidRole_ThenSetsRoleCorrectly(EmployeeRole role)
    {
        // Arrange & Act
        var employee = CreateValidEmployee(role: role);

        // Assert
        Assert.Equal(role, employee.Role);
    }

    [Fact]
    public void Constructor_WhenEmptyPhoneList_ThenAcceptsEmptyList()
    {
        // Arrange
        var emptyPhones = new List<Phone>();

        // Act
        var employee = CreateValidEmployee(phones: emptyPhones);

        // Assert
        Assert.Empty(employee.Phones);
    }

    [Fact]
    public void Constructor_WhenMultiplePhones_ThenAcceptsAllPhones()
    {
        // Arrange
        var phones = new List<Phone>
        {
            CreateValidPhone("11999999999"),
            CreateValidPhone("11888888888"),
            CreateValidPhone("11777777777")
        };

        // Act
        var employee = CreateValidEmployee(phones: phones);

        // Assert
        Assert.Equal(3, employee.Phones.Count);
        Assert.Equal(phones, employee.Phones);
    }

    [Fact]
    public void Constructor_WhenWeakPassword_ThenIsNotValid()
    {
        // Arrange
        var weakPassword = "123";

        // Act
        var employee = CreateValidEmployee(password: weakPassword);

        // Assert
        Assert.False(employee.IsValid);
        Assert.Contains(employee.Notifications, n => n.Contains("Password must"));
    }

    [Fact]
    public void Employee_WhenCreated_ThenHasUniqueId()
    {
        // Arrange & Act
        var employee1 = CreateValidEmployee();
        var employee2 = CreateValidEmployee();

        // Assert
        Assert.NotEqual(employee1.Id, employee2.Id);
        Assert.NotEqual(Guid.Empty, employee1.Id);
        Assert.NotEqual(Guid.Empty, employee2.Id);
    }

    [Fact]
    public void Employee_WhenCreated_ThenHasTimestamps()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var employee = CreateValidEmployee();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(employee.CreatedAt >= beforeCreation);
        Assert.True(employee.CreatedAt <= afterCreation);
        Assert.True(employee.UpdatedAt >= beforeCreation);
        Assert.True(employee.UpdatedAt <= afterCreation);
        // Check if timestamps are close (within 1 second)
        Assert.True(Math.Abs((employee.CreatedAt - employee.UpdatedAt).TotalMilliseconds) < 1000);
    }
}