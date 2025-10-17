using Microsoft.Extensions.Logging;
using Moq;
using RssTech.Employee.Application.UnitTests.Common;
using RssTech.Employee.Application.UseCases.Employee.Get.Handler;

namespace RssTech.Employee.Application.UnitTests.UseCases.Employee.Get;

public class GetEmployeeHandlerTests : ApplicationTestBase
{
    private readonly GetEmployeeHandler _handler;
    private readonly Mock<ILogger<GetEmployeeHandler>> _specificMockLogger;

    public GetEmployeeHandlerTests()
    {
        _specificMockLogger = new Mock<ILogger<GetEmployeeHandler>>();
        _handler = new GetEmployeeHandler(
            MockEmployeeRepository.Object,
            _specificMockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_ThenReturnsSuccessResultWithEmployeeData()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = CreateValidGetEmployeeRequest(employeeId);
        var employee = CreateValidEmployee(employeeId);

        MockEmployeeRepository
            .Setup(x => x.GetById(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(employee.Id, result.Data.Id);
        Assert.Equal(employee.FirstName, result.Data.FirstName);
        Assert.Equal(employee.LastName, result.Data.LastName);
        Assert.Equal(employee.Email.Address, result.Data.Email);
    }

    [Fact]
    public async Task Handle_WhenEmployeeDoesNotExist_ThenReturnsErrorResult()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = CreateValidGetEmployeeRequest(employeeId);

        MockEmployeeRepository
            .Setup(x => x.GetById(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Employee)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Employee not found", result.Message);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ThenReturnsErrorResult()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = CreateValidGetEmployeeRequest(employeeId);

        MockEmployeeRepository
            .Setup(x => x.GetById(employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred while getting the employee", result.Message);
    }
}