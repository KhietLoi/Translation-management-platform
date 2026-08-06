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
                    cancellationToken: token);
            };

            // Global Rate Limiter
            options.GlobalLimiter = CreateGlobalLimiter(settings.Global);

            // Authentication Policies
            RegisterPolicy(options, "auth-login", settings.Login);
            RegisterPolicy(options, "auth-register", settings.Register);
            RegisterPolicy(options, "auth-forgot-password", settings.ForgotPassword);
            RegisterPolicy(options, "auth-refresh-token", settings.RefreshToken);
            RegisterPolicy(options, "auth-resend-verification-email", settings.ResendVerificationEmail);
        });

        return services;
    }

    private static void RegisterPolicy(
        RateLimiterOptions options,
        string policyName,
        HttpRateLimitOptions.HttpRateLimitPolicy policy)
    {
        options.AddPolicy(policyName, httpContext =>
        {
            return CreatePartition(httpContext, policy);
        });
    }

    private static PartitionedRateLimiter<HttpContext> CreateGlobalLimiter(
        HttpRateLimitOptions.HttpRateLimitPolicy policy)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext => CreatePartition(httpContext, policy));
    }

    private static RateLimitPartition<string> CreatePartition(
        HttpContext httpContext,
        HttpRateLimitOptions.HttpRateLimitPolicy policy)
    {
        var ip = GetClientIp(httpContext);

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => CreateFixedWindowOptions(policy));
    }

    private static FixedWindowRateLimiterOptions CreateFixedWindowOptions(
        HttpRateLimitOptions.HttpRateLimitPolicy policy)
    {
        return new FixedWindowRateLimiterOptions
        {
            PermitLimit = policy.PermitLimit,
            Window = TimeSpan.FromMinutes(policy.WindowMinutes),
            QueueLimit = policy.QueueLimit,
            AutoReplenishment = true
        };
    }

    private static string GetClientIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}