using MySolution.Api.StartupRegistrations;
using MySolution.Application.ServiceRegistration;
using MySolution.Infrastructure;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
//1. Logging:
builder.Host.UseLogging();

//2. Register services
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddCorsLayer(builder.Configuration)
    .AddSwaggerLayer()
    .AddControllersLayer()
    .AddAuthorizationLayer();

/*
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "USER_VIEW",
        policy => policy.RequireClaim(
            "permission",
            "USER_VIEW"));
});
*/

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
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () =>
{
    return Results.Redirect("/swagger");
});
app.Run();

/*using Microsoft.AspNetCore.Identity;
using MySolution.Domain.Entities;

var hasher = new PasswordHasher<User>();

var user = new User
{
    Username = "admin"
};

var hash = hasher.HashPassword(user, "Admin@123");

Console.WriteLine(hash);*/