using MediatR;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Template;


namespace MySolution.Application.Features.Auth.SendForgotPasswordEmail;

public class SendForgotPasswordEmailHandler : IRequestHandler<SendForgotPasswordEmailCommand>
{
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    private readonly IEmailService _emailService;
    private readonly ITokenSetting _tokenSettings;
    private readonly ITemplateRenderer _templateRenderer;

    public SendForgotPasswordEmailHandler
    (
        IApplicationUrlProvider applicationUrlProvider,
        IEmailService emailService,
        ITokenSetting tokenSetting,
        ITemplateRenderer templateRenderer
    )
    {
        _applicationUrlProvider = applicationUrlProvider;
        _emailService = emailService;
        _tokenSettings = tokenSetting;
        _templateRenderer = templateRenderer;
    }
    public async Task Handle(SendForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var resetUrl = _applicationUrlProvider.GetResetPasswordUrl(Uri.EscapeDataString((request.Message.Token)));
        var html = await _templateRenderer.RenderAsync(
            "ForgotPassword.html",
            new
            {
                user_name = request.Message.Username,
                reset_url = resetUrl,
                expiry_minutes = _tokenSettings.PasswordResetExpiryMinutes,
                logo_url = "https://wmtstorageaccdevsa.blob.core.windows.net/documents/logo.png",
                year = DateTime.Now.Year
            }, cancellationToken);
        await _emailService.SendEmailAsync(request.Message.Email, "Reset Password", html, cancellationToken);
    }
}