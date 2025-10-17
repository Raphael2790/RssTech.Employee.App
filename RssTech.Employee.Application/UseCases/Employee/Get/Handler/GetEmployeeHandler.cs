using MediatR;
using Microsoft.Extensions.Logging;
using RssTech.Employee.Application.UseCases.Employee.Get.Request;
using RssTech.Employee.Application.UseCases.Employee.Get.Response;
using RssTech.Employee.Common.Contracts;
using RssTech.Employee.Domain.Interfaces.Repositories;

namespace RssTech.Employee.Application.UseCases.Employee.Get.Handler;

public sealed class GetEmployeeHandler(
    IEmployeeRepository employeeRepository,
    ILogger<GetEmployeeHandler> logger)
    : IRequestHandler<GetEmployeeRequest, Result<GetEmployeeResponse>>
{
    public async Task<Result<GetEmployeeResponse>> Handle(GetEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing get employee request for ID: {EmployeeId}", request.Id);

            var employee = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);

            if (employee is null)
            {
                logger.LogWarning("Employee with ID: {EmployeeId} not found", request.Id);
                return Result<GetEmployeeResponse>.Error("Employee not found");
            }

            var response = new GetEmployeeResponse
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email.Address,
                Role = employee.Role,
                DateOfBirth = employee.DateOfBirth,
                ManagerName = employee.ManagerName
            };

            logger.LogInformation("Employee with ID: {EmployeeId} found successfully", request.Id);
            return Result<GetEmployeeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting employee with ID: {EmployeeId}", request.Id);
            return Result<GetEmployeeResponse>.Error("An error occurred while getting the employee");
        }
    }
}