using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace RssTech.Employee.Api.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        // Limpa configurações existentes do Serilog
        Log.CloseAndFlush();

        var elasticsearchUri = builder.Configuration.GetConnectionString("Elasticsearch") ?? "http://localhost:9200";
        var environment = builder.Environment.EnvironmentName;
        var applicationName = builder.Environment.ApplicationName;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.Elasticsearch(ConfigureElasticsearchSink(elasticsearchUri, environment, applicationName))
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    private static ElasticsearchSinkOptions ConfigureElasticsearchSink(string elasticsearchUri, string environment, string applicationName)
    {
        return new ElasticsearchSinkOptions(new Uri(elasticsearchUri))
        {
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            IndexFormat = $"employee-logs-{environment.ToLowerInvariant()}-{{0:yyyy.MM.dd}}",
            NumberOfShards = 2,
            NumberOfReplicas = environment.Equals("Production", StringComparison.OrdinalIgnoreCase) ? 1 : 0,
            ModifyConnectionSettings = x => x.BasicAuthentication("", ""), // Configurar se necessário
            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                              EmitEventFailureHandling.WriteToFailureSink |
                              EmitEventFailureHandling.RaiseCallback,
            FailureCallback = (logEvent, exception) => Console.WriteLine($"Falha ao enviar log para Elasticsearch: {logEvent.MessageTemplate} - Erro: {exception?.Message}"),
            DeadLetterIndexName = $"employee-logs-{environment.ToLowerInvariant()}-failures",
            RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway,
            OverwriteTemplate = false,
            DetectElasticsearchVersion = true,
            CustomFormatter = new Serilog.Formatting.Elasticsearch.ElasticsearchJsonFormatter()
        };
    }

    public static void EnsureSerilogClosed()
    {
        Log.CloseAndFlush();
    }
}