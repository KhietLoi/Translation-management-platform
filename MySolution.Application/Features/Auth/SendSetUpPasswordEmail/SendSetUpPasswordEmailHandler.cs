using MediatR;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;


namespace MySolution.Application.Features.Auth.SendSetUpPasswordEmail;

public class SendSetUpPasswordEmailHandler : IRequestHandler<SendSetUpPasswordEmailCommand>
{
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    private readonly IEmailService _emailService;
    private readonly ITokenSetting _tokenSettings;
    private readonly ITemplateRenderer _templateRenderer;

    public SendSetUpPasswordEmailHandler
    (
        IEmailService emailService, 
        IApplicationUrlProvider applicationUrlProvider, 
        ITokenSetting tokenSetting,
        ITemplateRenderer templateRenderer
    )
    {
        _emailService = emailService;
        _applicationUrlProvider = applicationUrlProvider;
        _tokenSettings = tokenSetting;
        _templateRenderer = templateRenderer;
    }

    public async Task Handle(SendSetUpPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var setupPasswordUrl = _applicationUrlProvider.GetResetPasswordUrl(request.Message.Token);
        var html = await _templateRenderer.RenderAsync(
            "SetUpPassword.html",
            new
            {
                user_name = request.Message.Username,
                setup_password_url = setupPasswordUrl,
                expiry_minutes = _tokenSettings.PasswordResetExpiryMinutes,
                logo_url = "https://wmtstorageaccdevsa.blob.core.windows.net/documents/logo.png",
                year = DateTime.Now.Year
            }, cancellationToken);
        
        await _emailService.SendEmailAsync(request.Message.Email, "Set Up Your Password", html, cancellationToken);
    }
}