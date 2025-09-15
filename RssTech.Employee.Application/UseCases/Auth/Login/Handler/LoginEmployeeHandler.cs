using MediatR;
using Microsoft.Extensions.Logging;
using RssTech.Employee.Application.UseCases.Auth.Login.Request;
using RssTech.Employee.Application.UseCases.Auth.Login.Response;
using RssTech.Employee.Common.Contracts;
using RssTech.Employee.Common.Utils;
using RssTech.Employee.Domain.Interfaces.Repositories;
using RssTech.Employee.Domain.Interfaces.Services;

namespace RssTech.Employee.Application.UseCases.Auth.Login.Handler;

public sealed class LoginEmployeeHandler(
    IEmployeeRepository employeeRepository,
    IJwtTokenService jwtTokenService,
    ILogger<LoginEmployeeHandler> logger)
    : IRequestHandler<LoginEmployeeRequest, Result<LoginEmployeeResponse>>
{
    public async Task<Result<LoginEmployeeResponse>> Handle(LoginEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing login request for email: {Email}", request.Email);

            var employee = await employeeRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (employee == null)
            {
                logger.LogWarning("Employee not found with email: {Email}", request.Email);
                return Result<LoginEmployeeResponse>.Error("Invalid email or password");
            }

            var hashedPassword = PasswordHashGenerator.HashPassword(request.Password);
            if (employee.Password != hashedPassword)
            {
                logger.LogWarning("Invalid password for employee: {Email}", request.Email);
                return Result<LoginEmployeeResponse>.Error("Invalid email or password");
            }

            var token = jwtTokenService.GenerateToken(employee);
            var refreshToken = jwtTokenService.GenerateRefreshToken();
            var expiresAt = jwtTokenService.GetTokenExpiration();

            logger.LogInformation("Login successful for employee: {Email}", request.Email);

            var response = new LoginEmployeeResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };

            return Result<LoginEmployeeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing login for email: {Email}", request.Email);
            return Result<LoginEmployeeResponse>.Error("An error occurred during login");
        }
    }
}