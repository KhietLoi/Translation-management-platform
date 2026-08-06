using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;

namespace MySolution.Email.Application.Common.Bases;

public abstract class BaseEmailHandler
{
    protected readonly IApplicationUrlProvider UrlProvider;
    protected readonly IEmailTemplateFactory EmailTemplateFactory;
    protected readonly IEmailTemplateService EmailTemplateService;

    protected BaseEmailHandler(
        IApplicationUrlProvider urlProvider,
        IEmailTemplateFactory emailTemplateFactory,
        IEmailTemplateService emailTemplateService)
    {
        UrlProvider = urlProvider;
        EmailTemplateFactory = emailTemplateFactory;
        EmailTemplateService = emailTemplateService;
    }

    protected Task SendTemplateAsync(
        EmailType emailType,
        string email,
        string userName,
        string actionUrl,
        int expiryMinutes,
        CancellationToken cancellationToken)
    {
        var model = EmailTemplateFactory.Create(userName, actionUrl, expiryMinutes);
        
        return EmailTemplateService.SendAsync( emailType, email, model, cancellationToken);
    }
}