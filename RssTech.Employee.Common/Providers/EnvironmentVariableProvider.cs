using RssTech.Employee.Common.Providers.Interfaces;

namespace RssTech.Employee.Common.Providers;

public sealed class EnvironmentVariableProvider : IEnvironmentVariableProvider
{
    public string GetEnvironmentVariable(string key)
        => Environment.GetEnvironmentVariable(key) ?? string.Empty;
}
