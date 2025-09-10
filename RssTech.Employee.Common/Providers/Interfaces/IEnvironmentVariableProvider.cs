namespace RssTech.Employee.Common.Providers.Interfaces;

public interface IEnvironmentVariableProvider
{
    string GetEnvironmentVariable(string key);
}
