namespace RssTech.Employee.Domain.Enums;

internal enum EmployeeRole
{
    Employee = 1,
    Manager = 2,
    Director = 3,
    //Admins can not be created via API
    Administrator = 4
}
