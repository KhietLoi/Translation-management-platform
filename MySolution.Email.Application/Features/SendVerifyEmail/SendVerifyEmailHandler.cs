using MediatR;
using MySolution.Email.Application.Common.Bases;
using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Application.Common.Models;


namespace MySolution.Email.Application.Features.SendVerifyEmail;
public class SendVerifyEmailHandler : BaseEmailHandler, IRequestHandler<SendVerifyEmailCommand>
{
    private readonly ITokenSetting _tokenSettings;
    private readonly IEmailTemplateFactory<StandardEmailTemplateData, EmailTemplateModel> _emailTemplateFactory;

    public SendVerifyEmailHandler(
        IApplicationUrlProvider urlProvider,
        IEmailTemplateService emailTemplateService,
        ITokenSetting tokenSetting,
        IEmailTemplateFactory<StandardEmailTemplateData, EmailTemplateModel> emailTemplateFactory)
        : base(
            urlProvider,
            emailTemplateService)
    {
        _tokenSettings = tokenSetting;
        _emailTemplateFactory = emailTemplateFactory;   
    }

  
    public Task Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var data = new StandardEmailTemplateData
        {
            UserName = request.Message.Username,
            ActionUrl = UrlProvider.GetVerifyEmailUrl(request.Message.Token),
            ExpiryMinutes = _tokenSettings.EmailVerificationExpiryMinutes
        };

        var model = _emailTemplateFactory.Create(data);
        return SendTemplateAsync(EmailType.VerifyEmail, request.Message.Email, model, cancellationToken);
    }
}