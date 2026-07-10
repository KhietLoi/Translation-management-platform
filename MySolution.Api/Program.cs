using Microsoft.EntityFrameworkCore;
using MySolution.Api.StartupRegistrations;
using MySolution.Application.ServiceRegistration;
using MySolution.Infrastructure;
using MySolution.Infrastructure.Persistence;
using MySolution.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddSwaggerLayer()
    .AddControllersLayer()
    .AddCorsLayer(builder.Configuration);


var app = builder.Build();
// 3. Middleware pipeline
//app.UseRequestLocalizationLayer();
//app.UseSwaggerLayer();
//app.UseSerilogRequestLogging();

//app.UseRequestLogging();
//app.UseExceptionLayer();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await context.Database.MigrateAsync();

    await AppDbSeeder.SeedAsync(context);
}
app.UseSwaggerLayer();
app.UseHttpsRedirection();
app.UseCors("_allowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () =>
{
    return Results.Redirect("/swagger");
});
app.Run();