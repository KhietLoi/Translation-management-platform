using MediatR;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Templates;
using MySolution.Application.Constants;

namespace MySolution.Application.Features.Auth.SendVerifyEmail;

public class SendVerifyEmailHandler : IRequestHandler<SendVerifyEmailCommand>
{
    private readonly IEmailService _emailService;
    private readonly IApplicationUrlProvider _applicationUrlProvider;

    public SendVerifyEmailHandler(IEmailService emailService, IApplicationUrlProvider applicationUrlProvider)
    {
        _emailService = emailService;
        _applicationUrlProvider = applicationUrlProvider;
    }

    public async Task Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(10000, cancellationToken); 
        
        var verifyUrl = _applicationUrlProvider.GetVerifyEmailUrl(request.Message.Token);
        var html = EmailTemplateVerifyRegister.VerifyEmail(
            request.Message.Username,
            verifyUrl,
            AuthConstants.EmailVerificationExpiryMinutes
        );
        
        await _emailService.SendEmailAsync(request.Message.Email, "Verify Your Email", html, cancellationToken);
    }
}