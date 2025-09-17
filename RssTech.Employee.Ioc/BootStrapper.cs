using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using RssTech.Employee.Common.Providers;
using RssTech.Employee.Common.Providers.Interfaces;
using RssTech.Employee.Domain.Interfaces.Repositories;
using RssTech.Employee.Domain.Interfaces.Services;
using RssTech.Employee.Infrastructure.Context;
using RssTech.Employee.Infrastructure.Repositories;
using RssTech.Employee.Infrastructure.Services;
using RssTech.Employee.Ioc.Extensions;

namespace RssTech.Employee.Ioc;

public static class BootStrapper
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureSwagger()
                .AddProviders()
                .AddAppContext()
                .AddRepositories()
                .AddServices()
                .AddMediatR()
                .AddJwtAuthentication(configuration)
                .AddHttpContextAccessor()
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
        app.UseAuthentication();
        app.UseAuthorization();
    }

    private static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.TryAddScoped<IEnvironmentVariableProvider, EnvironmentVariableProvider>();
        services.TryAddScoped<IUrlProvider, UrlProvider>();

        return services;
    }

    private static IServiceCollection AddAppContext(this IServiceCollection services)
    {
        var databaseUrl = EnvironmentVariableProvider.Instance.GetEnvironmentVariable("DATABASE_URL");
        services.AddNpgsql<AppDbContext>(databaseUrl, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.TryAddScoped<IEmployeeRepository, EmployeeRepository>();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.TryAddScoped<IJwtTokenService, JwtTokenService>();
        services.TryAddScoped<IHierarchyValidationService, HierarchyValidationService>();
        return services;
    }


    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
            Assembly.GetAssembly(typeof(RssTech.Employee.Application.UseCases.Auth.Login.Handler.LoginEmployeeHandler))!));
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"] ?? "default-secret-key-that-should-be-changed-in-production";
        var issuer = configuration["Jwt:Issuer"] ?? "RssTech.Employee.Api";
        var audience = configuration["Jwt:Audience"] ?? "RssTech.Employee.Client";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }
}
