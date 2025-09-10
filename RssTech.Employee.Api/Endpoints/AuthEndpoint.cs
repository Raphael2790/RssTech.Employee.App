using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RssTech.Employee.Application.UseCases.Auth.Login.Request;
using RssTech.Employee.Application.UseCases.Auth.Login.Response;
using RssTech.Employee.Common.Contracts;

namespace RssTech.Employee.Api.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", async Task<Results<Ok<Result<LoginEmployeeResponse>>, BadRequest<Result<LoginEmployeeResponse>>>> ([FromServices] IMediator mediator, [FromBody] LoginEmployeeRequest request) 
        =>
        {
            var result = await mediator.Send(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        })
        .WithName("Login")
        .AllowAnonymous()
        .WithOpenApi(op =>
        {
            op.Security.Clear();
            return op;
        });
    }
}
