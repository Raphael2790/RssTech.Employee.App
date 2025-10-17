using MediatR;
using RssTech.Employee.Application.UseCases.Employee.List.Response;
using RssTech.Employee.Common.Contracts;

namespace RssTech.Employee.Application.UseCases.Employee.List.Request;

public record struct ListEmployeesRequest(int PageNumber, int PageSize) : IRequest<Result<IEnumerable<ListEmployeesResponse>>>;