using Microsoft.Extensions.Logging;
using Moq;
using RssTech.Employee.Application.UnitTests.Common;
using RssTech.Employee.Application.UseCases.Employee.Create.Handler;
using RssTech.Employee.Domain.Enums;

namespace RssTech.Employee.Application.UnitTests.UseCases.Employee.Create;

public class CreateEmployeeHandlerTests : ApplicationTestBase
{
    private readonly CreateEmployeeHandler _handler;
    private readonly Mock<ILogger<CreateEmployeeHandler>> _specificMockLogger;

    public CreateEmployeeHandlerTests()
    {
        _specificMockLogger = new Mock<ILogger<CreateEmployeeHandler>>();
        _handler = new CreateEmployeeHandler(
            MockEmployeeRepository.Object,
            MockHierarchyValidationService.Object,
            MockHttpContextAccessor.Object,
            _specificMockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ThenReturnsSuccessResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest();
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.ExistsByDocumentNumber(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(request.FirstName, result.Data.FirstName);
        Assert.Equal(request.LastName, result.Data.LastName);
        Assert.Equal(request.Email, result.Data.Email);
        Assert.Equal(request.Role, result.Data.Role);
        MockEmployeeRepository.Verify(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoUserContext_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest();
        SetupHttpContext(null); // No user context

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Authentication required", result.Message);
        MockEmployeeRepository.Verify(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest();
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true, isAuthenticated: false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Authentication required", result.Message);
        MockEmployeeRepository.Verify(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserCannotCreateEmployee_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest(role: EmployeeRole.Manager);
        SetupAuthenticatedContext(EmployeeRole.Employee, canCreateEmployee: false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Insufficient privileges to create employee with this role", result.Message);
        MockEmployeeRepository.Verify(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest();
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Employee with this email already exists", result.Message);
        MockEmployeeRepository.Verify(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDocumentAlreadyExists_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest();
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.ExistsByDocumentNumber(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Employee with this document already exists", result.Message);
        MockEmployeeRepository.Verify(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInvalidEmployeeData_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest(
            firstName: "", // Invalid first name
            email: "invalid-email" // Invalid email format
        );
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.ExistsByDocumentNumber(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Validation failed", result.Message);
        MockEmployeeRepository.Verify(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest();
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.ExistsByDocumentNumber(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred while creating the employee", result.Message);
    }

    [Fact]
    public async Task Handle_WhenValidRequestWithTwoPhones_ThenCreatesEmployeeWithBothPhones()
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest(
            phoneNumber1: "11999999999",
            phoneNumber2: "11888888888"
        );
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.ExistsByDocumentNumber(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        RssTech.Employee.Domain.Entities.Employee? capturedEmployee = null;
        MockEmployeeRepository
            .Setup(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()))
            .Callback<RssTech.Employee.Domain.Entities.Employee, CancellationToken>((emp, _) => capturedEmployee = emp)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedEmployee);
        Assert.Equal(2, capturedEmployee.Phones.Count);
    }

    [Fact]
    public async Task Handle_WhenValidRequestWithManager_ThenCreatesEmployeeWithManager()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var request = CreateValidCreateEmployeeRequest(managerName: managerId.ToString());
        SetupAuthenticatedContext(EmployeeRole.Manager, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.ExistsByDocumentNumber(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        RssTech.Employee.Domain.Entities.Employee? capturedEmployee = null;
        MockEmployeeRepository
            .Setup(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()))
            .Callback<RssTech.Employee.Domain.Entities.Employee, CancellationToken>((emp, _) => capturedEmployee = emp)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedEmployee);
        Assert.Equal(managerId, capturedEmployee.ManagerName);
    }

    [Theory]
    [InlineData(EmployeeRole.Employee)]
    [InlineData(EmployeeRole.Manager)]
    [InlineData(EmployeeRole.Director)]
    public async Task Handle_WhenValidRequestWithDifferentRoles_ThenCreatesEmployeeWithCorrectRole(EmployeeRole role)
    {
        // Arrange
        var request = CreateValidCreateEmployeeRequest(role: role);
        SetupAuthenticatedContext(EmployeeRole.Director, canCreateEmployee: true);

        MockEmployeeRepository
            .Setup(x => x.ExistsByEmail(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.ExistsByDocumentNumber(request.DocumentNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        MockEmployeeRepository
            .Setup(x => x.Create(It.IsAny<RssTech.Employee.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(role, result.Data!.Role);
    }
}