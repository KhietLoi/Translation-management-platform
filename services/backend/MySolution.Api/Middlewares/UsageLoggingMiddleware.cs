using System.Diagnostics;
using MediatR;
using MySolution.Application.Common.Models;
using MySolution.Application.Features.UsageLog.Commands.CreateUsageLog;

namespace MySolution.Api.Middlewares;

public class UsageLoggingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<UsageLoggingMiddleware> _logger;

    public UsageLoggingMiddleware(RequestDelegate next, ILogger<UsageLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IMediator  mediator)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var apiKeyContext = context.Items["ApiKey"] as ApiKeyContext;
            if (apiKeyContext != null)
            {
                try
                {
                    await mediator.Send(
                        new CreateUsageLogCommand(
                            new CreateUsageLogRequest
                            {
                                ApiKeyId = apiKeyContext.ApiKeyId,
                                ApplicationId = apiKeyContext.ApplicationId,
                                Endpoint = context.Request.Path,
                                Method = context.Request.Method,
                                StatusCode = context.Response.StatusCode,
                                DurationMs = (int)stopwatch.ElapsedMilliseconds,
                                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                                UserAgent = context.Request.Headers.UserAgent.ToString()
                            }));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save API key usage.");
                }
            }
            else
            {
                _logger.LogWarning("API Key context is missing. Usage logging will not be performed.");
            }
        }
    }
}