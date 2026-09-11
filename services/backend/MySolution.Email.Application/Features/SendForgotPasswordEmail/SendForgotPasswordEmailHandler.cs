using MediatR;
using MySolution.Email.Application.Common.Bases;
using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Application.Common.Models;


namespace MySolution.Email.Application.Features.SendForgotPasswordEmail;

public class SendForgotPasswordEmailHandler : BaseEmailHandler, IRequestHandler<SendForgotPasswordEmailCommand>
{
    private readonly ITokenSetting _tokenSettings;
    private readonly IEmailTemplateFactory<StandardEmailTemplateData, EmailTemplateModel> _emailTemplateFactory;
    
    public SendForgotPasswordEmailHandler(
        IApplicationUrlProvider urlProvider,
        IEmailTemplateService emailTemplateService,
        ITokenSetting tokenSetting,
        IEmailTemplateFactory<StandardEmailTemplateData, EmailTemplateModel> emailTemplateFactory)
        : base(urlProvider,emailTemplateService)
    {
        _tokenSettings = tokenSetting;
        _emailTemplateFactory = emailTemplateFactory;
    }

    public Task Handle(SendForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var data = new StandardEmailTemplateData
        {
            UserName = request.Message.Username,
            ActionUrl = UrlProvider.GetResetPasswordUrl(Uri.EscapeDataString(request.Message.Token)),
            ExpiryMinutes = _tokenSettings.PasswordResetExpiryMinutes
        };

        var model = _emailTemplateFactory.Create(data);
        
        return SendTemplateAsync(EmailType.ForgotPassword, request.Message.Email, model, cancellationToken);
    }
}