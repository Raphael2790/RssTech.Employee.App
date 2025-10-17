using Microsoft.Extensions.Logging;
using Moq;
using RssTech.Employee.Application.UnitTests.Common;
using RssTech.Employee.Application.UseCases.Employee.Update.Handler;
using RssTech.Employee.Application.UseCases.Employee.Update.Request;
using RssTech.Employee.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RssTech.Employee.Application.UnitTests.UseCases.Employee.Update
{
    public class UpdateEmployeeHandlerTests : ApplicationTestBase
    {
        private readonly UpdateEmployeeHandler _handler;
        private readonly Mock<ILogger<UpdateEmployeeHandler>> _specificMockLogger;

        public UpdateEmployeeHandlerTests()
        {
            _specificMockLogger = new Mock<ILogger<UpdateEmployeeHandler>>();
            _handler = new UpdateEmployeeHandler(
                MockEmployeeRepository.Object,
                MockHierarchyValidationService.Object,
                MockHttpContextAccessor.Object,
                _specificMockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ThenReturnsSuccessResult()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var request = new UpdateEmployeeRequest
            {
                Id = employeeId,
                FirstName = "Jane",
                LastName = "Doe",
                Password = "NewPassword123!",
                PhoneNumber1 = "11888888888"
            };
            var employee = CreateValidEmployee(employeeId);

            SetupAuthenticatedContext(EmployeeRole.Manager);
            MockHierarchyValidationService.Setup(x => x.CanUpdateEmployee(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), employeeId)).Returns(true);
            MockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
            MockEmployeeRepository.Setup(x => x.Update(It.IsAny<Domain.Entities.Employee>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(request.FirstName, result.Data.FirstName);
            Assert.Equal(request.LastName, result.Data.LastName);
            MockEmployeeRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Employee>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmployeeNotFound_ThenReturnsErrorResult()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var request = new UpdateEmployeeRequest { Id = employeeId };

            SetupAuthenticatedContext(EmployeeRole.Manager);
            MockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Employee)null!);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Employee not found", result.Message);
        }

        [Fact]
        public async Task Handle_WhenUnauthorized_ThenReturnsErrorResult()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var request = new UpdateEmployeeRequest { Id = employeeId };

            SetupAuthenticatedContext(EmployeeRole.Employee, isAuthenticated: false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Authentication required", result.Message);
        }

        [Fact]
        public async Task Handle_WhenInsufficientPrivileges_ThenReturnsErrorResult()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var request = new UpdateEmployeeRequest { Id = employeeId };
            var employee = CreateValidEmployee(employeeId);

            SetupAuthenticatedContext(EmployeeRole.Employee);
            MockHierarchyValidationService.Setup(x => x.CanUpdateEmployee(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), employeeId)).Returns(false);
            MockEmployeeRepository.Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>())).ReturnsAsync(employee);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Insufficient privileges", result.Message);
        }
    }
}