using MySolution.Api.Middlewares;

namespace MySolution.Api.StartupRegistrations;

public static class RequestLogRegistration
{
    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }
}