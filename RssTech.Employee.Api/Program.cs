using RssTech.Employee.Api.Endpoints;
using RssTech.Employee.Api.Extensions;
using RssTech.Employee.Infrastructure.Extensions;
using RssTech.Employee.Ioc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog via extensão
builder.ConfigureSerilog();

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.AddGlobalExceptionHandler();

var app = builder.Build();

// Aplica migrations e executa seed se necessário
await app.ApplyMigrationsAndSeedAsync();

app.MapEmployeeEndpoints();
app.MapAuthEndpoints();

app.UseGlobalExceptionHandler();

app.UseServices(builder.Configuration);

try
{
    Log.Information("Iniciando aplicação Employee API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou durante a inicialização");
}
finally
{
    SerilogExtensions.EnsureSerilogClosed();
}
