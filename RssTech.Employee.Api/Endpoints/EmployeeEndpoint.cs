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

        group.MapPost("/", () => "Create a new employee")
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
