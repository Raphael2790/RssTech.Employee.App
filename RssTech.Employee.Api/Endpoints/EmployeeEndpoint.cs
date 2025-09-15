using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RssTech.Employee.Application.UseCases.Employee.Create.Request;
using RssTech.Employee.Application.UseCases.Employee.Create.Response;
using RssTech.Employee.Common.Contracts;

namespace RssTech.Employee.Api.Endpoints;

public static class EmployeeEndpoint
{
    public static void MapEmployeeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/employees")
            .WithTags("Employees");

        group.MapGet("/", () => "Get all employees")
            .WithName("GetAllEmployees")
            .RequireAuthorization();

        group.MapGet("/{id}", (int id) => $"Get employee with ID {id}")
            .WithName("GetEmployeeById")
            .RequireAuthorization();

        group.MapPost("/", async Task<Results<Ok<Result<CreateEmployeeResponse>>, BadRequest<Result<CreateEmployeeResponse>>>> ([FromServices] IMediator mediator, [FromBody] CreateEmployeeRequest request)
        =>
        {
            var result = await mediator.Send(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        })
        .WithName("CreateEmployee")
        .RequireAuthorization();

        group.MapPut("/{id}", (int id) => $"Update employee with ID {id}")
            .WithName("UpdateEmployee")
            .RequireAuthorization();

        group.MapDelete("/{id}", (int id) => $"Delete employee with ID {id}")
            .WithName("DeleteEmployee")
            .RequireAuthorization();
    }
}
