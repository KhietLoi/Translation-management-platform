using MediatR;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Templates;
using MySolution.Application.Constants;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.SendSetUpPasswordEmail;

public class SendSetUpPasswordEmailHandler : IRequestHandler<SendSetUpPasswordEmailCommand>
{
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    private readonly IEmailService _emailService;

    
    public SendSetUpPasswordEmailHandler(IEmailService emailService, IApplicationUrlProvider applicationUrlProvider)
    {
        _emailService = emailService;
        _applicationUrlProvider = applicationUrlProvider;
    }

    public async Task Handle(SendSetUpPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        
        // await Task.Delay(10000, cancellationToken); 
        var setupPasswordUrl = _applicationUrlProvider.GetResetPasswordUrl(request.Message.Token);

        var html = SetupPasswordTemplate.SetupPassword(
            request.Message.Username,
            setupPasswordUrl,
            AuthConstants.PasswordResetExpiryMinutes
        );
        await _emailService.SendEmailAsync(request.Message.Email, "Verify Your Email", html, cancellationToken);
    }
}