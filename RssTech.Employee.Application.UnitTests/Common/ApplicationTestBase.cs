using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RssTech.Employee.Application.UseCases.Auth.Login.Request;
using RssTech.Employee.Application.UseCases.Employee.Create.Request;
using RssTech.Employee.Application.UseCases.Employee.Get.Request;
using RssTech.Employee.Application.UseCases.Employee.List.Request;
using RssTech.Employee.Application.UseCases.Employee.Update.Request;
using RssTech.Employee.Domain.Enums;
using RssTech.Employee.Domain.Interfaces.Repositories;
using RssTech.Employee.Domain.Interfaces.Services;
using RssTech.Employee.Domain.ValueObjects;
using System.Security.Claims;

namespace RssTech.Employee.Application.UnitTests.Common;

public abstract class ApplicationTestBase
{
    protected readonly Mock<IEmployeeRepository> MockEmployeeRepository;
    protected readonly Mock<IHierarchyValidationService> MockHierarchyValidationService;
    protected readonly Mock<IJwtTokenService> MockJwtTokenService;
    protected readonly Mock<IHttpContextAccessor> MockHttpContextAccessor;
    protected readonly Mock<ILogger<object>> MockLogger;

    protected ApplicationTestBase()
    {
        MockEmployeeRepository = new Mock<IEmployeeRepository>();
        MockHierarchyValidationService = new Mock<IHierarchyValidationService>();
        MockJwtTokenService = new Mock<IJwtTokenService>();
        MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        MockLogger = new Mock<ILogger<object>>();
    }

    protected static CreateEmployeeRequest CreateValidCreateEmployeeRequest(
        string firstName = "John",
        string lastName = "Doe",
        string email = "john.doe@example.com",
        string password = "Password123!",
        string documentNumber = "123.456.789-01",
        string phoneNumber1 = "11999999999",
        string? phoneNumber2 = null,
        DateTime? dateOfBirth = null,
        EmployeeRole role = EmployeeRole.Employee,
        string? managerName = null)
    {
        return new CreateEmployeeRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password,
            DocumentNumber = documentNumber,
            PhoneNumber1 = phoneNumber1,
            PhoneNumber2 = phoneNumber2 ?? "",
            DateOfBirth = dateOfBirth ?? DateTime.Now.AddYears(-25),
            Role = role,
            ManagerName = managerName ?? ""
        };
    }

    protected static LoginEmployeeRequest CreateValidLoginRequest(
        string email = "john.doe@example.com",
        string password = "Password123!")
    {
        return new LoginEmployeeRequest
        {
            Email = email,
            Password = password
        };
    }

    protected static GetEmployeeRequest CreateValidGetEmployeeRequest(
        Guid? id = null)
    {
        return new GetEmployeeRequest
        {
            Id = id ?? Guid.NewGuid()
        };
    }

    protected static ListEmployeesRequest CreateValidListEmployeesRequest()
    {
        return new ListEmployeesRequest();
    }

    protected static UpdateEmployeeRequest CreateValidUpdateEmployeeRequest(
        string firstName = "John Updated",
        string lastName = "Doe Updated",
        string password = "Password123!",
        string phoneNumber1 = "11888888888",
        string? phoneNumber2 = null)
    {
        return new UpdateEmployeeRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Password = password,
            PhoneNumber1 = phoneNumber1,
            PhoneNumber2 = phoneNumber2
        };
    }

    protected static RssTech.Employee.Domain.Entities.Employee CreateValidEmployee(
        Guid? id = null,
        string firstName = "John",
        string lastName = "Doe",
        string email = "john.doe@example.com",
        string password = "hashedPassword",
        string documentNumber = "123.456.789-01",
        DateTime? dateOfBirth = null,
        EmployeeRole role = EmployeeRole.Employee)
    {
        var employee = new RssTech.Employee.Domain.Entities.Employee(
            firstName,
            lastName,
            new Email(email),
            password,
            new EmployeeDocument(documentNumber),
            [new Phone("11999999999")],
            dateOfBirth ?? DateTime.Now.AddYears(-25),
            role,
            null);

        if (id.HasValue)
        {
            // Use reflection to set the Id since it's protected
            var idProperty = employee.GetType().GetProperty("Id");
            idProperty?.SetValue(employee, id.Value);
        }

        return employee;
    }

    protected ClaimsPrincipal CreateAuthenticatedUser(
        EmployeeRole role = EmployeeRole.Manager,
        string email = "manager@example.com",
        string userId = "manager-id")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email),
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, role.ToString())
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

    protected void SetupHttpContext(ClaimsPrincipal? user = null)
    {
        var httpContext = new DefaultHttpContext();
        if (user != null)
        {
            httpContext.User = user;
        }
        MockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
    }

    protected void SetupAuthenticatedContext(
        EmployeeRole role = EmployeeRole.Manager,
        bool canCreateEmployee = true,
        bool isAuthenticated = true)
    {
        var user = CreateAuthenticatedUser(role);
        SetupHttpContext(user);

        MockHierarchyValidationService
            .Setup(x => x.IsAuthenticated(It.IsAny<ClaimsPrincipal>()))
            .Returns(isAuthenticated);

        MockHierarchyValidationService
            .Setup(x => x.CanCreateEmployee(It.IsAny<ClaimsPrincipal>(), It.IsAny<EmployeeRole>()))
            .Returns(canCreateEmployee);

        MockHierarchyValidationService
            .Setup(x => x.GetUserRole(It.IsAny<ClaimsPrincipal>()))
            .Returns(role);
    }
}