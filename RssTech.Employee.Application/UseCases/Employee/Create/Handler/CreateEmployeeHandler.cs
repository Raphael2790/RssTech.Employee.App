using MediatR;
using RssTech.Employee.Application.UseCases.Employee.Create.Request;
using RssTech.Employee.Application.UseCases.Employee.Create.Response;

namespace RssTech.Employee.Application.UseCases.Employee.Create.Handler;

public sealed class CreateEmployeeHandler()
    : IRequestHandler<CreateEmployeeRequest, CreateEmployeeResponse>
{
    public Task<CreateEmployeeResponse> Handle(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        
    }
}
