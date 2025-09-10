using RssTech.Employee.Api.Middlewares;

namespace RssTech.Employee.Api.Extensions;

public static class MiddlewaresExtensions
{
    public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlerMiddleware>();
        return services;
    }

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }
}
