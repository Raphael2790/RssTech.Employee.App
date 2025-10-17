using MediatR;
using Microsoft.Extensions.Logging;
using RssTech.Employee.Application.UseCases.Employee.List.Request;
using RssTech.Employee.Application.UseCases.Employee.List.Response;
using RssTech.Employee.Common.Contracts;
using RssTech.Employee.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RssTech.Employee.Application.UseCases.Employee.List.Handler
{
    public sealed class ListEmployeesHandler(
        IEmployeeRepository employeeRepository,
        ILogger<ListEmployeesHandler> logger)
        : IRequestHandler<ListEmployeesRequest, Result<IEnumerable<ListEmployeesResponse>>>
    {
        public async Task<Result<IEnumerable<ListEmployeesResponse>>> Handle(ListEmployeesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Processing list employees request");

                var employees = await employeeRepository.GetAll(cancellationToken);

                var response = employees.Select(e => new ListEmployeesResponse
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email.Address,
                    Role = e.Role
                });

                logger.LogInformation("Found {EmployeeCount} employees", response.Count());
                return Result<IEnumerable<ListEmployeesResponse>>.Success(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error listing employees");
                return Result<IEnumerable<ListEmployeesResponse>>.Error("An error occurred while listing the employees");
            }
        }
    }
}