using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Infrastructure.Authentication;
using MySolution.Infrastructure.BackgroundServices;
using MySolution.Infrastructure.MassTransit;
using MySolution.Infrastructure.MassTransit.Consumers;
using MySolution.Infrastructure.Options;
using MySolution.Infrastructure.Persistence;
using MySolution.Infrastructure.Services;
using Shared.MassTransit.Contracts;
using StackExchange.Redis;

namespace MySolution.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //db
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        //jwt:
        services.AddJwtAuthentication(configuration);
        services.AddCustomServices();
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        //Mail service:
        services.AddOptions<SendGridOptions>()
            .Bind(configuration.GetSection(SendGridOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<IHashService, HashService>();
        
        //Token Options: (Use for Token email)
        services.AddOptions<TokenOptions>()
            .Bind(configuration.GetSection(TokenOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<ITokenSetting, TokenSetting>();
        //Background Clean RefreshToken
        services.AddHostedService<RefreshTokenCleanupHostedService>();
        services.AddOptions<RefreshTokenCleanupOptions>()
            .Bind(configuration.GetSection(RefreshTokenCleanupOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        //Redis
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        var redisOptions = configuration
            .GetSection(RedisOptions.SectionName)
            .Get<RedisOptions>() ?? throw new InvalidOperationException("Redis configuration missing");
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            return ConnectionMultiplexer.Connect(
                redisOptions.ConnectionString);
        });
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
        //services.AddScoped<ITokenBlacklistService, FakeTokenBlacklistService>();
        
        //Frontend Url:
        services.AddOptions<FrontendOptions>()
            .Bind(configuration.GetSection(FrontendOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IApplicationUrlProvider, ApplicationUrlProvider>();
        
        //VerifyEmail:
        services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();
        //Reset-password:
        services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        //MassTransit:
        var rabbitMqOptions = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>() ?? throw new InvalidOperationException("RabbitMQ configuration missing");
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SendVerifyEmailConsumer>();
            x.AddConsumer<SendSetupPasswordEmailConsumer>();
            x.AddConsumer<SendForgotPasswordEmailConsumer>();
            //x.SetKebabCaseEndpointNameFormatter();
            
            x.UsingRabbitMq((context,cfg) =>
            {
               //cfg.UseRawJsonDeserializer();
               cfg.Host(rabbitMqOptions.Host, "/", h =>
               {
                   h.Username(rabbitMqOptions.Username);
                   h.Password(rabbitMqOptions.Password);
               });
               //cfg.ConfigureEndpoints(context);
               cfg.ReceiveEndpoint(QueueNames.VerifyEmail, e =>
               {
                   e.ConfigureConsumer<SendVerifyEmailConsumer>(context);
               });
               cfg.ReceiveEndpoint(QueueNames.SetupPasswordEmail, e =>
               {
                   e.ConfigureConsumer<SendSetupPasswordEmailConsumer>(context);
               });
               cfg.ReceiveEndpoint(QueueNames.ForgotPasswordEmail, e =>
               {
                   e.ConfigureConsumer<SendForgotPasswordEmailConsumer>(context);
               });
            });
        });
        
        services.AddScoped<IMessageSender, SendEndPointCustomProvider>();
        //AzureBlob:
        services.Configure<AzureBlobOptions>(configuration.GetSection(AzureBlobOptions.SectionName));
        services.AddScoped<IAzureBlobService, AzureBlobService>();
        return services;
    }
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        //HttpContext
        services.AddHttpContextAccessor();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}