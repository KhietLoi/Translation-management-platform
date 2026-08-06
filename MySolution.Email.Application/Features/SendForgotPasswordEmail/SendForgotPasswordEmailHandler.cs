using MediatR;
using MySolution.Email.Application.Common.Bases;
using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;


namespace MySolution.Email.Application.Features.SendForgotPasswordEmail;

public class SendForgotPasswordEmailHandler
    : BaseEmailHandler,
        IRequestHandler<SendForgotPasswordEmailCommand>
{
    private readonly ITokenSetting _tokenSettings;

    public SendForgotPasswordEmailHandler(
        IApplicationUrlProvider urlProvider,
        IEmailTemplateFactory emailTemplateFactory,
        IEmailTemplateService emailTemplateService,
        ITokenSetting tokenSetting)
        : base(
            urlProvider,
            emailTemplateFactory,
            emailTemplateService)
    {
        _tokenSettings = tokenSetting;
    }

    public Task Handle(SendForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        return SendTemplateAsync(
            EmailType.ForgotPassword,
            request.Message.Email,
            request.Message.Username,
            UrlProvider.GetResetPasswordUrl(
                Uri.EscapeDataString(request.Message.Token)),
            _tokenSettings.PasswordResetExpiryMinutes,
            cancellationToken);
    }
}