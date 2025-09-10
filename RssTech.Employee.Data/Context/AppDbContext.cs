using Microsoft.EntityFrameworkCore;

namespace RssTech.Employee.Infrastructure.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<Domain.Entities.Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            optionsBuilder.UseNpgsql(databaseUrl);
        }

        optionsBuilder.EnableSensitiveDataLogging();
    }
}
