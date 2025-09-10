namespace RssTech.Employee.Common.Providers.Interfaces;

public interface IUrlProvider
{
    string GetBaseUrl();
    string GetEmployeeEndpoint();
    string GetEmployeeByIdEndpoint(Guid employeeId);
}
