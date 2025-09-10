using MediatR;
using RssTech.Employee.Application.UseCases.Auth.Login.Response;
using RssTech.Employee.Common.Contracts;

namespace RssTech.Employee.Application.UseCases.Auth.Login.Request;

public record struct LoginEmployeeRequest : IRequest<Result<LoginEmployeeResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
