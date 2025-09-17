using RssTech.Employee.Api.Endpoints;
using RssTech.Employee.Api.Extensions;
using RssTech.Employee.Infrastructure.Extensions;
using RssTech.Employee.Ioc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

builder.Services.AddGlobalExceptionHandler();

var app = builder.Build();

// Aplica migrations e executa seed se necessário
await app.ApplyMigrationsAndSeedAsync();

app.MapEmployeeEndpoints();
app.MapAuthEndpoints();

app.UseGlobalExceptionHandler();

app.UseServices(builder.Configuration);

app.Run();
