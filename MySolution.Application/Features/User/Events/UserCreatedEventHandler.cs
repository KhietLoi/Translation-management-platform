using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Templates;

namespace MySolution.Application.Features.User.Events;

public sealed class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly IEmailService _emailService;

    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(IEmailService emailService, ILogger<UserCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
        
       
    }
    
    
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "UserCreatedEventHandler started for {Email}",
            notification.Email);
        try
        {
            var html = EmailTemplateBuilder.WelcomeEmail(notification.UserName);
            await _emailService.SendEmailAsync(
                notification.Email,
                "Welcome To MySolution",
                html,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", notification.Email);
        }
    }
}