using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RssTech.Employee.Common.Providers;
using RssTech.Employee.Common.Providers.Interfaces;
using RssTech.Employee.Ioc.Extensions;

namespace RssTech.Employee.Ioc;

public static class BootStrapper
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureSwagger()
                .AddProviders()
                .AddOpenApi();

        return services;
    }

    public static void UseServices(this WebApplication app, IConfiguration configuration)
    {
        app.UseSwaggerInterface(configuration);

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
    }

    private static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.TryAddScoped<IEnvironmentVariableProvider, EnvironmentVariableProvider>();
        services.TryAddScoped<IUrlProvider, UrlProvider>();

        return services;
    }
}
