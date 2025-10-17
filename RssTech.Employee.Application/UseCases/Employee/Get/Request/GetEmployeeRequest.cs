using MediatR;
using RssTech.Employee.Application.UseCases.Employee.Get.Response;
using RssTech.Employee.Common.Contracts;

namespace RssTech.Employee.Application.UseCases.Employee.Get.Request;

public record struct GetEmployeeRequest(Guid Id) : IRequest<Result<GetEmployeeResponse>>;