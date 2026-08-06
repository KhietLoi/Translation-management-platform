using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Email.Application.Common.Factories;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Infrastructure.MassTransit.Registration;
using MySolution.Email.Infrastructure.Options;
using MySolution.Email.Infrastructure.Services;

namespace MySolution.Email.Infrastructure;

public static class DependencyInjection 
{
    public static IServiceCollection AddEmailInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SendGridOptions>()
            .Bind(configuration.GetSection(SendGridOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<IApplicationUrlProvider, ApplicationUrlProvider>();
        //Token Options: (Use for Token email)
        services.AddOptions<TokenOptions>()
            .Bind(configuration.GetSection(TokenOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<ITokenSetting, TokenSetting>();
        //services.AddScoped<IHashService, HashService>();
        //Frontend Url:
        services.AddOptions<FrontendOptions>()
            .Bind(configuration.GetSection(FrontendOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        //Scriban
        services.AddScoped<ITemplateRenderer, ScribanTemplateRenderer>();
        //MassTransit:
        services.AddMassTransitServices(configuration);
        //Email common:
        services.AddScoped<IEmailTemplateFactory, EmailTemplateFactory>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        return services;
    }
}