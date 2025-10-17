using Microsoft.Extensions.Logging;
using Moq;
using RssTech.Employee.Application.UnitTests.Common;
using RssTech.Employee.Application.UseCases.Auth.Login.Handler;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RssTech.Employee.Application.UnitTests.UseCases.Auth.Login
{
    public class LoginEmployeeHandlerTests : ApplicationTestBase
    {
        private readonly LoginEmployeeHandler _handler;
        private readonly Mock<ILogger<LoginEmployeeHandler>> _specificMockLogger;

        public LoginEmployeeHandlerTests()
        {
            _specificMockLogger = new Mock<ILogger<LoginEmployeeHandler>>();
            _handler = new LoginEmployeeHandler(
                MockEmployeeRepository.Object,
                MockJwtTokenService.Object,
                _specificMockLogger.Object);
        }

        [Fact]
        public async Task Handle_WhenValidCredentials_ThenReturnsSuccessResultWithToken()
        {
            // Arrange
            var request = CreateValidLoginRequest();
            var employee = CreateValidEmployee(email: request.Email); // Password will be hashed in handler

            MockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            MockJwtTokenService
                .Setup(x => x.GenerateToken(It.IsAny<Domain.Entities.Employee>()))
                .Returns("valid-token");
            MockJwtTokenService
                .Setup(x => x.GenerateRefreshToken())
                .Returns("valid-refresh-token");
            MockJwtTokenService
                .Setup(x => x.GetTokenExpiration())
                .Returns(DateTime.UtcNow.AddHours(1));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("valid-token", result.Data.Token);
            Assert.Equal("valid-refresh-token", result.Data.RefreshToken);
        }

        [Fact]
        public async Task Handle_WhenEmployeeNotFound_ThenReturnsErrorResult()
        {
            // Arrange
            var request = CreateValidLoginRequest();

            MockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Employee)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public async Task Handle_WhenInvalidPassword_ThenReturnsErrorResult()
        {
            // Arrange
            var request = CreateValidLoginRequest(password: "wrong-password");
            var employee = CreateValidEmployee(email: request.Email); // Correct password is "Password123!"

            MockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ThenReturnsErrorResult()
        {
            // Arrange
            var request = CreateValidLoginRequest();

            MockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("An error occurred during login", result.Message);
        }
    }
}