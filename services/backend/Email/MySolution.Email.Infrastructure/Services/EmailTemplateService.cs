using MySolution.Email.Application.Common.Definitions;
using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly ITemplateRenderer _renderer;
    private readonly IEmailService _emailService;

    public EmailTemplateService(
        ITemplateRenderer renderer,
        IEmailService emailService)
    {
        _renderer = renderer;
        _emailService = emailService;
    }

    /*public async Task SendAsync(
        EmailType emailType,
        string email,
        EmailTemplateModel model,
        CancellationToken cancellationToken)
    {
        var definition = EmailTemplateDefinitions.Get(emailType);

        var html = await _renderer.RenderAsync(
                definition.Template,
                new
                {
                    user_name = model.UserName,
                    action_url = model.ActionUrl,
                    expiry_minutes = model.ExpiryMinutes,
                    logo_url = model.LogoUrl,
                    year = model.Year
                },
                cancellationToken);

        await _emailService.SendEmailAsync(email, definition.Subject, html, cancellationToken);
    }*/

    public async Task SendAsync<TModel>(EmailType emailType, string email, TModel model, CancellationToken cancellationToken = default) where TModel : class
    {
        var definition = EmailTemplateDefinitions.Get(emailType);
        var html = await _renderer.RenderAsync(definition.Template, model, cancellationToken);
        await _emailService.SendEmailAsync(email, definition.Subject, html, cancellationToken );
    }
}