using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RssTech.Employee.Application.UseCases.Employee.Update.Request;
using RssTech.Employee.Application.UseCases.Employee.Update.Response;
using RssTech.Employee.Common.Contracts;
using RssTech.Employee.Common.Utils;
using RssTech.Employee.Domain.Interfaces.Repositories;
using RssTech.Employee.Domain.Interfaces.Services;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Application.UseCases.Employee.Update.Handler;

public sealed class UpdateEmployeeHandler(
    IEmployeeRepository employeeRepository,
    IHierarchyValidationService hierarchyValidationService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<UpdateEmployeeHandler> logger)
    : IRequestHandler<UpdateEmployeeRequest, Result<UpdateEmployeeResponse>>
{
    public async Task<Result<UpdateEmployeeResponse>> Handle(UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing update employee request for ID: {EmployeeId}", request.Id);

            var currentUser = httpContextAccessor.HttpContext?.User;

            if (currentUser is null || !hierarchyValidationService.IsAuthenticated(currentUser))
            {
                logger.LogWarning("Unauthorized attempt to update employee ID: {EmployeeId}", request.Id);
                return Result<UpdateEmployeeResponse>.Error("Authentication required");
            }

            var employeeToUpdate = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);
            if (employeeToUpdate is null)
            {
                logger.LogWarning("Employee with ID: {EmployeeId} not found for update", request.Id);
                return Result<UpdateEmployeeResponse>.Error("Employee not found");
            }

            if (!hierarchyValidationService.CanUpdateEmployee(currentUser, employeeToUpdate.Id))
            {
                logger.LogWarning("User attempted to update employee without sufficient privileges. Employee ID: {EmployeeId}", request.Id);
                return Result<UpdateEmployeeResponse>.Error("Insufficient privileges");
            }

            employeeToUpdate.Update(request.FirstName, request.LastName);

            if (!string.IsNullOrEmpty(request.Password))
            {
                var hashedPassword = PasswordHashGenerator.HashPassword(request.Password);
                employeeToUpdate.UpdatePassword(hashedPassword);
            }

            var phones = new List<Phone>();
            if (!string.IsNullOrEmpty(request.PhoneNumber1))
                phones.Add(new Phone(request.PhoneNumber1));
            if (!string.IsNullOrEmpty(request.PhoneNumber2))
                phones.Add(new Phone(request.PhoneNumber2));
            employeeToUpdate.UpdatePhones(phones);

            if (!employeeToUpdate.IsValid)
            {
                var errors = string.Join(", ", employeeToUpdate.Notifications);
                logger.LogWarning("Employee update validation failed for ID: {EmployeeId}. Errors: {Errors}", request.Id, errors);
                return Result<UpdateEmployeeResponse>.Error($"Validation failed: {errors}");
            }

            await employeeRepository.Update(employeeToUpdate, cancellationToken);
            logger.LogInformation("Employee with ID: {EmployeeId} updated successfully", request.Id);

            var response = new UpdateEmployeeResponse
            {
                Id = employeeToUpdate.Id,
                FirstName = employeeToUpdate.FirstName,
                LastName = employeeToUpdate.LastName,
                Email = employeeToUpdate.Email.Address
            };

            return Result<UpdateEmployeeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating employee with ID: {EmployeeId}", request.Id);
            return Result<UpdateEmployeeResponse>.Error("An error occurred while updating the employee");
        }
    }
}