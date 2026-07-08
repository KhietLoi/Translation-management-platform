using System.Diagnostics;
using System.Text;
using MediatR;
using MySolution.Api.Attributes;
using Shared.Helpers;

namespace MySolution.Api.Middlewares;

public class RequestLoggingMiddleware
{
       private readonly RequestDelegate _next;
    private readonly IMediator _mediator;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        IMediator mediator,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var shouldLog = endpoint?.Metadata.GetMetadata<EnableRequestLoggingAttribute>() != null;
        if (!shouldLog)
        {
            await _next(context);
            return;
        }
        
        var sw = Stopwatch.StartNew();
        var requestId = Guid.NewGuid();

        context.Request.EnableBuffering();

        string body = null;
        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true);

            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        string errorMessage = null;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            context.Response.StatusCode = 500;
            throw;
        }
        finally
        {
            sw.Stop();

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);

            try
            {
                /*await _mediator.Send(new CreateRequestLogCommand(new CreateRequestLogRequest
                {
                    RequestId = requestId,
                    Path = context.Request.Path,
                    Method = context.Request.Method,
                    Headers = JsonHelper.Serialize(context.Request.Headers),
                    QueryParams = JsonHelper.Serialize(context.Request.Query),
                    Body = string.IsNullOrEmpty(body) ? null : body,
                    ResponseStatus = context.Response.StatusCode,
                    ResponseBody = string.IsNullOrEmpty(responseBody) ? null : responseBody,
                    DurationMs = (int)sw.ElapsedMilliseconds,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers.UserAgent,
                    ErrorMessage = errorMessage,
                }));*/
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Failed to save request log");
            }
        }
    }
}