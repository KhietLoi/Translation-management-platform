using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Templates;
using MySolution.Application.Constants;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Auth.Events;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    private readonly IEmailVerificationTokenService  _emailVerificationTokenService;

    public UserRegisteredEventHandler
    (
        ILogger<UserRegisteredEventHandler> logger,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IApplicationUrlProvider applicationUrlProvider,
        IEmailVerificationTokenService emailVerificationTokenService
    )
    {
        _logger = logger;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _applicationUrlProvider = applicationUrlProvider;
        _emailVerificationTokenService = emailVerificationTokenService;
    }
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    { 
        var functionName = $"{nameof(UserRegisteredEventHandler)} =>";
        _logger.LogInformation("{FunctionName} Start processing email verification.", functionName);
        _logger.LogInformation("Start Send Email");
        //await Task.Delay(30000, cancellationToken);
        _logger.LogInformation("End Send Email");
        
        try
        {
            var token = _emailVerificationTokenService.GenerateVerificationToken(notification.UserId, notification.Email);
            var verifyUrl = _applicationUrlProvider.GetVerifyEmailUrl(token);
            //Email template:
            var html = EmailTemplateVerifyRegister.VerifyEmail(notification.Username, verifyUrl, AuthConstants.EmailVerificationExpiryMinutes);
            await _emailService.SendEmailAsync(notification.Email, "Verify your email", html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} is failed");
        }
    }
}