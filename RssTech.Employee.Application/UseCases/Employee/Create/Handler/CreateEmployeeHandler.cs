using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RssTech.Employee.Application.UseCases.Employee.Create.Request;
using RssTech.Employee.Application.UseCases.Employee.Create.Response;
using RssTech.Employee.Common.Contracts;
using RssTech.Employee.Common.Utils;
using RssTech.Employee.Domain.Interfaces.Repositories;
using RssTech.Employee.Domain.Interfaces.Services;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Application.UseCases.Employee.Create.Handler;

public sealed class CreateEmployeeHandler(
    IEmployeeRepository employeeRepository,
    IHierarchyValidationService hierarchyValidationService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<CreateEmployeeHandler> logger)
    : IRequestHandler<CreateEmployeeRequest, Result<CreateEmployeeResponse>>
{
    public async Task<Result<CreateEmployeeResponse>> Handle(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing create employee request for email: {Email}", request.Email);

            // Validar autenticação e autorização hierárquica
            var currentUser = httpContextAccessor.HttpContext?.User;

            if (currentUser is null)
            {
                logger.LogWarning("No user context found");
                return Result<CreateEmployeeResponse>.Error("Authentication required");
            }

            if (!hierarchyValidationService.IsAuthenticated(currentUser))
            {
                logger.LogWarning("Unauthorized attempt to create employee");
                return Result<CreateEmployeeResponse>.Error("Authentication required");
            }

            if (!hierarchyValidationService.CanCreateEmployee(currentUser, request.Role))
            {
                var currentUserRole = hierarchyValidationService.GetUserRole(currentUser);
                logger.LogWarning("User with role {CurrentRole} attempted to create employee with role {TargetRole}",
                    currentUserRole, request.Role);
                return Result<CreateEmployeeResponse>.Error("Insufficient privileges to create employee with this role");
            }

            // Verificar se email já existe
            var emailExists = await employeeRepository.ExistsByEmail(request.Email, cancellationToken);
            if (emailExists)
            {
                logger.LogWarning("Attempt to create employee with existing email: {Email}", request.Email);
                return Result<CreateEmployeeResponse>.Error("Employee with this email already exists");
            }

            // Verificar se documento já existe
            var documentExists = await employeeRepository.ExistsByDocumentNumber(request.DocumentNumber, cancellationToken);
            if (documentExists)
            {
                logger.LogWarning("Attempt to create employee with existing document: {Document}", request.DocumentNumber);
                return Result<CreateEmployeeResponse>.Error("Employee with this document already exists");
            }

            // Criar value objects
            var email = new Email(request.Email);
            var document = new EmployeeDocument(request.DocumentNumber);
            var phones = new List<Phone>();

            if (!string.IsNullOrEmpty(request.PhoneNumber1))
                phones.Add(new Phone(request.PhoneNumber1));

            if (!string.IsNullOrEmpty(request.PhoneNumber2))
                phones.Add(new Phone(request.PhoneNumber2));

            // Hash da senha
            var hashedPassword = PasswordHashGenerator.HashPassword(request.Password);

            // Criar entidade Employee
            var employee = new Domain.Entities.Employee(
                request.FirstName,
                request.LastName,
                email,
                hashedPassword,
                document,
                phones,
                request.DateOfBirth,
                request.Role,
                string.IsNullOrEmpty(request.ManagerName) ? null : Guid.Parse(request.ManagerName)
            );

            // Validar entidade
            if (!employee.IsValid)
            {
                var errors = string.Join(", ", employee.Notifications);
                logger.LogWarning("Employee validation failed: {Errors}", errors);
                return Result<CreateEmployeeResponse>.Error($"Validation failed: {errors}");
            }

            // Salvar no repositório
            await employeeRepository.Create(employee, cancellationToken);

            logger.LogInformation("Employee created successfully with ID: {EmployeeId}", employee.Id);

            var response = new CreateEmployeeResponse
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email.Address,
                Role = employee.Role
            };

            return Result<CreateEmployeeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating employee for email: {Email}", request.Email);
            return Result<CreateEmployeeResponse>.Error("An error occurred while creating the employee");
        }
    }
}
