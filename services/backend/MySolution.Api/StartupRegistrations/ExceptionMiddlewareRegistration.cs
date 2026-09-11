using MySolution.Api.Middlewares;

namespace MySolution.Api.StartupRegistrations;

public static class ExceptionMiddlewareRegistration
{
    public static WebApplication UseExceptionLayer(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}