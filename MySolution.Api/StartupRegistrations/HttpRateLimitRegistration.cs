using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using MySolution.Api.Options;


namespace MySolution.Api.StartupRegistrations;

public static class HttpRateLimitRegistration
{
    public static IServiceCollection AddHttpRateLimit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<HttpRateLimitOptions>(configuration.GetSection(HttpRateLimitOptions.SectionName));

        var settings = configuration
            .GetSection(HttpRateLimitOptions.SectionName)
            .Get<HttpRateLimitOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{HttpRateLimitOptions.SectionName}' not found.");

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/json";

                await context.HttpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        success = false,
                        message = "Too many requests. Please try again later."
                    },
                    token);
            };

            AddPolicy(options, "http-login", settings.Login);
            AddPolicy(options, "http-register", settings.Register);
            AddPolicy(options, "http-forgot-password", settings.ForgotPassword);
            AddPolicy(options, "http-refresh-token", settings.RefreshToken);
        });

        return services;
    }

    private static void AddPolicy(
        RateLimiterOptions options,
        string policyName,
        HttpRateLimitOptions.HttpRateLimitPolicy policy)
    {
        options.AddPolicy(policyName, httpContext =>
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString()
                     ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ip,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    Window = TimeSpan.FromMinutes(policy.WindowMinutes),
                    QueueLimit = policy.QueueLimit,
                    AutoReplenishment = true
                });
        });
    }
}