using Microsoft.Extensions.Logging;
using Moq;
using RssTech.Employee.Application.UnitTests.Common;
using RssTech.Employee.Application.UseCases.Employee.List.Handler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RssTech.Employee.Application.UnitTests.UseCases.Employee.List
{
    public class ListEmployeesHandlerTests : ApplicationTestBase
    {
        private readonly ListEmployeesHandler _handler;
        private readonly Mock<ILogger<ListEmployeesHandler>> _specificMockLogger;

        public ListEmployeesHandlerTests()
        {
            _specificMockLogger = new Mock<ILogger<ListEmployeesHandler>>();
            _handler = new ListEmployeesHandler(
                MockEmployeeRepository.Object,
                _specificMockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenEmployeesExist_ThenReturnsSuccessResultWithEmployeeList()
        {
            // Arrange
            var request = CreateValidListEmployeesRequest();
            var employees = new List<Domain.Entities.Employee>
            {
                CreateValidEmployee(id: Guid.NewGuid(), email: "employee1@example.com"),
                CreateValidEmployee(id: Guid.NewGuid(), email: "employee2@example.com")
            };

            MockEmployeeRepository
                .Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(employees);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task Handle_WhenNoEmployeesExist_ThenReturnsSuccessResultWithEmptyList()
        {
            // Arrange
            var request = CreateValidListEmployeesRequest();
            var employees = new List<Domain.Entities.Employee>();

            MockEmployeeRepository
                .Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(employees);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ThenReturnsErrorResult()
        {
            // Arrange
            var request = CreateValidListEmployeesRequest();

            MockEmployeeRepository
                .Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("An error occurred while listing the employees", result.Message);
        }
    }
}