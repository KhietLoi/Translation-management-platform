using Destructurama;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using MySolution.Application.Options;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
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
        if (elk?.Enabled == true)
        {
            if (!Uri.TryCreate(
                    elk.ElasticSearchUrl,
                    UriKind.Absolute,
                    out var elasticsearchUri)
                || (elasticsearchUri.Scheme != Uri.UriSchemeHttp
                    && elasticsearchUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException("Logging:Elk:ElasticSearchUrl must be a valid HTTP(S) URL.");
            }

            loggerConfiguration.WriteTo.Elasticsearch(
                new[] { elasticsearchUri },
                options =>
                {
                    options.DataStream = new DataStreamName(
                        "logs",
                        elk.DataStream,
                        environmentName.ToLowerInvariant());

                    options.BootstrapMethod = BootstrapMethod.Failure;
                });
        }
        // if (elk.Enabled)
        //     loggerConfiguration.WriteTo.Elasticsearch(ConfigureElasticSink(elk.ElasticSearchUrl, environmentName));

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
}