using MediatR;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Templates;
using MySolution.Application.Constants;

namespace MySolution.Application.Features.Auth.SendForgotPasswordEmail;

public class SendForgotPasswordEmailHandler : IRequestHandler<SendForgotPasswordEmailCommand>
{
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    private readonly IEmailService _emailService;

    public SendForgotPasswordEmailHandler(IApplicationUrlProvider applicationUrlProvider, IEmailService emailService)
    {
        _applicationUrlProvider = applicationUrlProvider;
        _emailService = emailService;
    }
    public async Task Handle(SendForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var resetUrl = _applicationUrlProvider.GetResetPasswordUrl(Uri.EscapeDataString((request.Message.Token)));
        var html = ResetPasswordTemplate.ResetPassword(request.Message.Username, resetUrl, AuthConstants.PasswordResetExpiryMinutes);
        await _emailService.SendEmailAsync(request.Message.Email, "Reset Password", html, cancellationToken);
    }
}