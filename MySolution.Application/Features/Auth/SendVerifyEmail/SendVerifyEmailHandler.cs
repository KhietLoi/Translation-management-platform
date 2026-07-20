using MediatR;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Templates;
using MySolution.Application.Constants;

namespace MySolution.Application.Features.Auth.SendVerifyEmail;

public class SendVerifyEmailHandler : IRequestHandler<SendVerifyEmailCommand>
{
    private readonly IEmailService _emailService;
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    private readonly ITokenSetting _tokenSettings;

    public SendVerifyEmailHandler(IEmailService emailService, IApplicationUrlProvider applicationUrlProvider,  ITokenSetting tokenSetting)
    {
        _emailService = emailService;
        _applicationUrlProvider = applicationUrlProvider;
        _tokenSettings = tokenSetting;
    }

    public async Task Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(10000, cancellationToken); 
        
        var verifyUrl = _applicationUrlProvider.GetVerifyEmailUrl(request.Message.Token);
        var html = EmailTemplateVerifyRegister.VerifyEmail(
            request.Message.Username,
            verifyUrl,  
            _tokenSettings.EmailVerificationExpiryMinutes
        );
        
        await _emailService.SendEmailAsync(request.Message.Email, "Verify Your Email", html, cancellationToken);
    }
}