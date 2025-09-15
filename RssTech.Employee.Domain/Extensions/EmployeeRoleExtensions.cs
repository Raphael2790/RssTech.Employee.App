using RssTech.Employee.Domain.Enums;

namespace RssTech.Employee.Domain.Extensions;

public static class EmployeeRoleExtensions
{
    /// <summary>
    /// Verifica se um cargo pode criar/gerenciar outro cargo baseado na hierarquia
    /// Employee (1) < Manager (2) < Director (3) < Administrator (4)
    /// Só pode criar cargos de nível inferior ou igual
    /// </summary>
    public static bool CanManage(this EmployeeRole managerRole, EmployeeRole targetRole)
    {
        // Administradores podem gerenciar todos
        if (managerRole == EmployeeRole.Administrator)
            return true;

        // Não pode criar administradores via API
        if (targetRole == EmployeeRole.Administrator)
            return false;

        // Só pode criar cargos de nível inferior
        return (int)managerRole > (int)targetRole;
    }

    /// <summary>
    /// Obtém os cargos que um determinado cargo pode criar
    /// </summary>
    public static IEnumerable<EmployeeRole> GetManagedRoles(this EmployeeRole role)
    {
        return role switch
        {
            EmployeeRole.Administrator => [EmployeeRole.Employee, EmployeeRole.Manager, EmployeeRole.Director],
            EmployeeRole.Director => [EmployeeRole.Employee, EmployeeRole.Manager],
            EmployeeRole.Manager => [EmployeeRole.Employee],
            EmployeeRole.Employee => [],
            _ => Array.Empty<EmployeeRole>()
        };
    }

    /// <summary>
    /// Verifica se um cargo tem permissão hierárquica para realizar operações administrativas
    /// </summary>
    public static bool HasAdministrativePrivileges(this EmployeeRole role)
    {
        return role >= EmployeeRole.Manager;
    }
}