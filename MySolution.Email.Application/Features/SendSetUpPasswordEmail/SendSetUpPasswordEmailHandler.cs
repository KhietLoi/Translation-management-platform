using MediatR;
using MySolution.Email.Application.Common.Bases;
using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;


namespace MySolution.Email.Application.Features.SendSetUpPasswordEmail;

public class SendSetUpPasswordEmailHandler
    : BaseEmailHandler,
        IRequestHandler<SendSetUpPasswordEmailCommand>
{
    private readonly ITokenSetting _tokenSettings;

    public SendSetUpPasswordEmailHandler(
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

    public Task Handle(SendSetUpPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        return SendTemplateAsync(
            EmailType.SetUpPassword,
            request.Message.Email,
            request.Message.Username,
            UrlProvider.GetResetPasswordUrl(request.Message.Token),
            _tokenSettings.PasswordResetExpiryMinutes,
            cancellationToken);
    }
}