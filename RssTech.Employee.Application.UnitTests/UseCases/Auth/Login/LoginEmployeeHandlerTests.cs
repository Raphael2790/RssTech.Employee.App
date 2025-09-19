using Microsoft.Extensions.Logging;
using Moq;
using RssTech.Employee.Application.UnitTests.Common;
using RssTech.Employee.Application.UseCases.Auth.Login.Handler;
using RssTech.Employee.Domain.Enums;

namespace RssTech.Employee.Application.UnitTests.UseCases.Auth.Login;

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
    public async Task Handle_WhenValidCredentials_ThenReturnsSuccessWithToken()
    {
        // Arrange
        var request = CreateValidLoginRequest();
        var hashedPassword = RssTech.Employee.Common.Utils.PasswordHashGenerator.HashPassword("Password123!");
        var employee = CreateValidEmployee(password: hashedPassword);
        var expectedToken = "jwt-token";
        var expectedRefreshToken = "refresh-token";
        var expectedExpiresAt = DateTime.UtcNow.AddHours(1);

        MockEmployeeRepository
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        MockJwtTokenService
            .Setup(x => x.GenerateToken(employee))
            .Returns(expectedToken);

        MockJwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns(expectedRefreshToken);

        MockJwtTokenService
            .Setup(x => x.GetTokenExpiration())
            .Returns(expectedExpiresAt);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(expectedToken, result.Data.Token);
        Assert.Equal(expectedRefreshToken, result.Data.RefreshToken);
        Assert.Equal(expectedExpiresAt, result.Data.ExpiresAt);
        MockJwtTokenService.Verify(x => x.GenerateToken(employee), Times.Once);
        MockJwtTokenService.Verify(x => x.GenerateRefreshToken(), Times.Once);
        MockJwtTokenService.Verify(x => x.GetTokenExpiration(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmployeeNotFound_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidLoginRequest();

        MockEmployeeRepository
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RssTech.Employee.Domain.Entities.Employee?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password", result.Message);
        MockJwtTokenService.Verify(x => x.GenerateToken(It.IsAny<RssTech.Employee.Domain.Entities.Employee>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInvalidPassword_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidLoginRequest(password: "wrongPassword");
        var correctHashedPassword = RssTech.Employee.Common.Utils.PasswordHashGenerator.HashPassword("Password123!");
        var employee = CreateValidEmployee(password: correctHashedPassword);

        MockEmployeeRepository
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password", result.Message);
        MockJwtTokenService.Verify(x => x.GenerateToken(It.IsAny<RssTech.Employee.Domain.Entities.Employee>()), Times.Never);
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
        MockJwtTokenService.Verify(x => x.GenerateToken(It.IsAny<RssTech.Employee.Domain.Entities.Employee>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenJwtServiceThrowsException_ThenReturnsErrorResult()
    {
        // Arrange
        var request = CreateValidLoginRequest();
        var hashedPassword = RssTech.Employee.Common.Utils.PasswordHashGenerator.HashPassword("Password123!");
        var employee = CreateValidEmployee(password: hashedPassword);

        MockEmployeeRepository
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        MockJwtTokenService
            .Setup(x => x.GenerateToken(employee))
            .Throws(new Exception("JWT generation error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred during login", result.Message);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("manager@company.com")]
    [InlineData("director@organization.org")]
    public async Task Handle_WhenValidCredentialsWithDifferentEmails_ThenReturnsSuccess(string email)
    {
        // Arrange
        var request = CreateValidLoginRequest(email: email);
        var hashedPassword = RssTech.Employee.Common.Utils.PasswordHashGenerator.HashPassword("Password123!");
        var employee = CreateValidEmployee(email: email, password: hashedPassword);
        var expectedToken = "jwt-token";
        var expectedRefreshToken = "refresh-token";
        var expectedExpiresAt = DateTime.UtcNow.AddHours(1);

        MockEmployeeRepository
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        MockJwtTokenService
            .Setup(x => x.GenerateToken(employee))
            .Returns(expectedToken);

        MockJwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns(expectedRefreshToken);

        MockJwtTokenService
            .Setup(x => x.GetTokenExpiration())
            .Returns(expectedExpiresAt);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        MockEmployeeRepository.Verify(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(EmployeeRole.Employee)]
    [InlineData(EmployeeRole.Manager)]
    [InlineData(EmployeeRole.Director)]
    [InlineData(EmployeeRole.Administrator)]
    public async Task Handle_WhenValidCredentialsWithDifferentRoles_ThenReturnsSuccess(EmployeeRole role)
    {
        // Arrange
        var request = CreateValidLoginRequest();
        var hashedPassword = RssTech.Employee.Common.Utils.PasswordHashGenerator.HashPassword("Password123!");
        var employee = CreateValidEmployee(password: hashedPassword, role: role);
        var expectedToken = "jwt-token";
        var expectedRefreshToken = "refresh-token";
        var expectedExpiresAt = DateTime.UtcNow.AddHours(1);

        MockEmployeeRepository
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        MockJwtTokenService
            .Setup(x => x.GenerateToken(employee))
            .Returns(expectedToken);

        MockJwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns(expectedRefreshToken);

        MockJwtTokenService
            .Setup(x => x.GetTokenExpiration())
            .Returns(expectedExpiresAt);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        MockJwtTokenService.Verify(x => x.GenerateToken(employee), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidCredentials_ThenLogsInformationMessages()
    {
        // Arrange
        var request = CreateValidLoginRequest();
        var hashedPassword = RssTech.Employee.Common.Utils.PasswordHashGenerator.HashPassword("Password123!");
        var employee = CreateValidEmployee(password: hashedPassword);
        var expectedToken = "jwt-token";
        var expectedRefreshToken = "refresh-token";
        var expectedExpiresAt = DateTime.UtcNow.AddHours(1);

        MockEmployeeRepository
            .Setup(x => x.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        MockJwtTokenService
            .Setup(x => x.GenerateToken(employee))
            .Returns(expectedToken);

        MockJwtTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns(expectedRefreshToken);

        MockJwtTokenService
            .Setup(x => x.GetTokenExpiration())
            .Returns(expectedExpiresAt);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _specificMockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing login request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _specificMockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Login successful")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}