using MediatR;
using MySolution.Email.Application.Common.Bases;
using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;


namespace MySolution.Email.Application.Features.SendVerifyEmail;
public class SendVerifyEmailHandler : BaseEmailHandler, IRequestHandler<SendVerifyEmailCommand>
{
    private readonly ITokenSetting _tokenSettings;

    public SendVerifyEmailHandler(
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

    public Task Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
    {
        return SendTemplateAsync(
            EmailType.VerifyEmail,
            request.Message.Email,
            request.Message.Username,
            UrlProvider.GetVerifyEmailUrl(request.Message.Token),
            _tokenSettings.EmailVerificationExpiryMinutes,
            cancellationToken);
    }
}