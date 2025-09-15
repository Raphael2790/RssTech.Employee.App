using Microsoft.EntityFrameworkCore;
using RssTech.Employee.Infrastructure.Context;

namespace RssTech.Employee.Infrastructure.Factory;

public sealed class AppDbContextFactory : IDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        optionsBuilder.UseNpgsql(databaseUrl);
        return new AppDbContext(optionsBuilder.Options);
    }
}
