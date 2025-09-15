using RssTech.Employee.Domain.Enums;
using System.Security.Claims;

namespace RssTech.Employee.Domain.Interfaces.Services;

public interface IHierarchyValidationService
{
    bool CanCreateEmployee(ClaimsPrincipal currentUser, EmployeeRole targetRole);
    EmployeeRole GetUserRole(ClaimsPrincipal user);
    Guid GetUserId(ClaimsPrincipal user);
    bool IsAuthenticated(ClaimsPrincipal user);
}