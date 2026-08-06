using Microsoft.AspNetCore.DataProtection;
using MySolution.Api.Middlewares;
using MySolution.Api.StartupRegistrations;
using MySolution.Api.StartupRegistrations.Swagger;
using MySolution.Application.ServiceRegistration;
using MySolution.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//1. Logging:
builder.Host.UseLogging();
//2. Rate Limit:
builder.Services.AddHttpRateLimit(builder.Configuration);
//3. Register services
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddCorsLayer(builder.Configuration)
    .AddSwaggerLayer()
    .AddControllersLayer()
    .AddAuthorizationLayer()
    .AddDataProtection().SetApplicationName("MySolution");

var app = builder.Build();

// 3. Middleware pipeline

app.UseSwaggerLayer();
app.UseSerilogRequestLogging();
app.UseCors("_allowSpecificOrigins");

/*app.UseRequestLogging();*/
app.UseExceptionLayer();

/*
// 4. Init localization service
app.InitLocalization();
*/

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
// ApiKey Usage
app.UseMiddleware<UsageLoggingMiddleware>();

app.MapControllers();
app.MapGet("/", () => { return Results.Redirect("/swagger"); });
app.Run();