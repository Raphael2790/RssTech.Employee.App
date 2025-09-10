using RssTech.Employee.Common.Providers.Interfaces;

namespace RssTech.Employee.Common.Providers;

public sealed class UrlProvider(IEnvironmentVariableProvider environmentVariableProvider) 
    : IUrlProvider
{
    public string GetBaseUrl()
    {
        var baseUrl = environmentVariableProvider.GetEnvironmentVariable("BASE_URL");
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Base URL is not configured.");

        return baseUrl;
    }

    public string GetEmployeeByIdEndpoint(Guid employeeId)
    {
        return $"{GetEmployeeEndpoint()}/{employeeId}";
    }

    public string GetEmployeeEndpoint()
    {
        return $"{GetBaseUrl()}api/employees";
    }
}
