using Microsoft.EntityFrameworkCore;
using MySolution.Api.StartupRegistrations;
using MySolution.Application.ServiceRegistration;
using MySolution.Infrastructure;
using MySolution.Infrastructure.Persistence;
using MySolution.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

//Register Services:
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddSwaggerLayer()
    .AddControllersLayer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await context.Database.MigrateAsync();

    await AppDbSeeder.SeedAsync(context);
}

app.UseSwaggerLayer();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () =>
{
    return Results.Redirect("/swagger");
});
/*app.MapGet("/", () => "Hello World!");*/
app.Run();