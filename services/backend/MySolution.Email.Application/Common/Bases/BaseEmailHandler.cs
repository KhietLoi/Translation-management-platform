using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;

namespace MySolution.Email.Application.Common.Bases;

public abstract class BaseEmailHandler
{
    protected readonly IApplicationUrlProvider UrlProvider;
    protected readonly IEmailTemplateService EmailTemplateService;

    protected BaseEmailHandler(
        IApplicationUrlProvider urlProvider,
        IEmailTemplateService emailTemplateService)
    {
        UrlProvider = urlProvider;
        EmailTemplateService = emailTemplateService;
    }

    protected Task SendTemplateAsync <TModel>(
        EmailType emailType,
        string email,
        TModel model,
        CancellationToken cancellationToken) where TModel : class
    {
        return EmailTemplateService.SendAsync(emailType, email, model, cancellationToken);
    }
}