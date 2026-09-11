


using MySolution.Email.Application.ServiceRegistration;
using MySolution.Email.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
//1. Logging:
//builder.Host.UseLogging();

//2. Register services
builder.Services
    .AddEmailApplication(builder.Configuration)
    .AddEmailInfrastructure(builder.Configuration);
  
   // .AddCorsLayer(builder.Configuration)
    //.AddSwaggerLayer()
    //.AddControllersLayer()
   // .AddAuthorizationLayer()
   // .AddDataProtection().SetApplicationName("MySolution");

var app = builder.Build();

// 3. Middleware pipeline

/*
app.UseSwaggerLayer();
app.UseSerilogRequestLogging();*/
//app.UseCors("_allowSpecificOrigins");

/*app.UseRequestLogging();*/
/*app.UseExceptionLayer();*/

/*
// 4. Init localization service
app.InitLocalization();
*/

//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();
app.MapGet("/", () =>
{
    return Results.Redirect("/swagger");
});
app.Run();