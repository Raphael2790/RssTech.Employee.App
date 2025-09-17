using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RssTech.Employee.Common.Utils;
using RssTech.Employee.Domain.Enums;
using RssTech.Employee.Domain.ValueObjects;
using RssTech.Employee.Infrastructure.Context;

namespace RssTech.Employee.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // Verifica se há migrations pendentes
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found.");
            }

            // Executa seed apenas se não existir administrador
            await SeedInitialDataAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization.");
            throw;
        }

        return app;
    }

    private static async Task SeedInitialDataAsync(AppDbContext context, ILogger logger)
    {
        // Verifica se já existe um administrador
        var adminExists = await context.Employees
            .AnyAsync(e => e.Role == EmployeeRole.Administrator);

        if (!adminExists)
        {
            logger.LogInformation("Creating default administrator employee...");

            var adminEmployee = new Domain.Entities.Employee(
                firstName: "System",
                lastName: "Administrator",
                email: new Email("admin@rsstech.com"),
                password: PasswordHashGenerator.HashPassword("Admin@123"),
                document: new EmployeeDocument("00000000000"),
                phones: [new Phone("+55 11 99999-9999")],
                dateOfBirth: new DateTime(1990, 1, 1),
                role: EmployeeRole.Administrator
            );

            await context.Employees.AddAsync(adminEmployee);
            await context.SaveChangesAsync();

            logger.LogInformation("Default administrator created successfully. Email: admin@rsstech.com, Password: Admin@123");
        }
        else
        {
            logger.LogInformation("Administrator employee already exists. Skipping seed.");
        }
    }
}