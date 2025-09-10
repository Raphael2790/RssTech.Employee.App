using RssTech.Employee.Api.Endpoints;
using RssTech.Employee.Api.Extensions;
using RssTech.Employee.Ioc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices();

builder.Services.AddGlobalExceptionHandler();

var app = builder.Build();

app.MapEmployeeEndpoints();
app.MapAuthEndpoints();

app.UseGlobalExceptionHandler();

app.UseServices(builder.Configuration);

app.Run();
