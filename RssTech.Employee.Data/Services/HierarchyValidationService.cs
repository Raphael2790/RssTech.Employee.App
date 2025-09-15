using RssTech.Employee.Domain.Enums;
using RssTech.Employee.Domain.Extensions;
using RssTech.Employee.Domain.Interfaces.Services;
using System.Security.Claims;

namespace RssTech.Employee.Infrastructure.Services;

public sealed class HierarchyValidationService : IHierarchyValidationService
{
    public bool CanCreateEmployee(ClaimsPrincipal currentUser, EmployeeRole targetRole)
    {
        if (!IsAuthenticated(currentUser))
            return false;

        var currentUserRole = GetUserRole(currentUser);
        return currentUserRole.CanManage(targetRole);
    }

    public EmployeeRole GetUserRole(ClaimsPrincipal user)
    {
        var roleValue = user.FindFirst("employee_role")?.Value;

        if (string.IsNullOrEmpty(roleValue) || !int.TryParse(roleValue, out int roleInt))
            return EmployeeRole.Employee; // Default to lowest privilege

        return Enum.IsDefined(typeof(EmployeeRole), roleInt)
            ? (EmployeeRole)roleInt
            : EmployeeRole.Employee;
    }

    public Guid GetUserId(ClaimsPrincipal user)
    {
        var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdValue, out Guid userId) ? userId : Guid.Empty;
    }

    public bool IsAuthenticated(ClaimsPrincipal user)
    {
        return user?.Identity?.IsAuthenticated == true;
    }
}