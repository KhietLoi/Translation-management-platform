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

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger, IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    { 
        var functionName = $"{nameof(UserRegisteredEventHandler)} =>";
        _logger.LogInformation("{FunctionName} Start processing email verification.", functionName);
        
        try
        {
            var token = Guid.NewGuid().ToString("N");

            var verificationToken = new EmailVerificationToken
            {
                Id = Guid.NewGuid(),
                UserId = notification.UserId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(AuthConstants.EmailVerificationExpiryHours),
                IsUsed = false
            };
            
            //Save Token:
            await _unitOfWork.EmailVerificationToken.Add(verificationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            var verifyUrl = $"http://localhost:5173/verify-email?token={token}";
            //Email template:
            var html = EmailTemplateVerifyRegister.VerifyEmail( notification.Username, verifyUrl, AuthConstants.EmailVerificationExpiryHours);

            await _emailService.SendEmailAsync(notification.Email, "Verify your email", html, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}