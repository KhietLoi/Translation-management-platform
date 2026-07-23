using Destructurama;
using MySolution.Application.Options;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Extensions;

namespace MySolution.Api.StartupRegistrations;

public static class CustomLogging
{
    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
        {
            var appOptions = context.Configuration.GetOptions<AppOptions>(AppOptions.OptionName);
            var loggingOptions = context.Configuration.GetOptions<LoggingOptions>(LoggingOptions.OptionName);
            var applicationName = appOptions.Name;
            var environmentName = context.HostingEnvironment.EnvironmentName;

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration, "Logging")
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithDemystifiedStackTraces()
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Destructure.UsingAttributes();
            Configure(loggerConfiguration, environmentName, loggingOptions, applicationName);
        });

        return hostBuilder;
    }

    private static void Configure(LoggerConfiguration loggerConfiguration, string environmentName,
        LoggingOptions loggingOptions, string applicationName)
    {
        var title = $"[{applicationName}_{environmentName}] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}";
        var elk = loggingOptions.Elk;
        if (elk.Enabled)
            loggerConfiguration.WriteTo.Elasticsearch(ConfigureElasticSink(elk.ElasticSearchUrl, environmentName));

        var seq = loggingOptions.Seq;
        if (seq.Enabled) loggerConfiguration.WriteTo.Seq(seq.Url, apiKey: seq.ApiKey);

        var teams = loggingOptions.MicrosoftTeams;
        if (teams.Enabled)
            loggerConfiguration.WriteTo.MicrosoftTeams(teams.WebHookUri, title, teams.BatchSizeLimit,
                TimeSpan.FromSeconds(teams.Period), restrictedToMinimumLevel: LogEventLevel.Error);

        if (teams.EnabledCriticalLevel)
            loggerConfiguration.WriteTo.MicrosoftTeams(teams.WebHookUriCriticalLevel, title, teams.BatchSizeLimit,
                restrictedToMinimumLevel: LogEventLevel.Fatal);

        if (loggingOptions.ConsoleEnabled)
        {
            if (environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase))
                loggerConfiguration.WriteTo.Console(
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{Properties:j}] {ExceptionEvent} {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code);
            else
                loggerConfiguration.WriteTo.Console(new RenderedCompactJsonFormatter(new JsonValueFormatter(null)));
        }
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(string elasticSearchUrl, string environment)
    {
        var options = new ElasticsearchSinkOptions(new Uri(elasticSearchUrl))
        {
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
            TemplateName = $"application-logs-{environment}",
            OverwriteTemplate = true,
            IndexFormat = $"{environment.ToLower()}-{DateTime.UtcNow:yyyy.MM.dd}",
            DetectElasticsearchVersion = true,
            RegisterTemplateFailure = RegisterTemplateRecovery.IndexAnyway,
            TypeName = null,
            BatchAction = ElasticOpType.Create,
            FailureCallback = (logEvent, ex) =>
                Console.WriteLine("Unable to submit event " + logEvent.MessageTemplate + " Error: " + ex.Message),
            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog | EmitEventFailureHandling.RaiseCallback
        };

        return options;
    }
}