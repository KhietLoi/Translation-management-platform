using MySolution.Api.StartupRegistrations;
using MySolution.Application.ServiceRegistration;
using MySolution.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//Register Services:
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddSwaggerLayer()
    .AddControllersLayer();

var app = builder.Build();

app.UseSwaggerLayer();
app.MapControllers();
app.Run();