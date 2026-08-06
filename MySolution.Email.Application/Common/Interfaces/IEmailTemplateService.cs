using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Common.Interfaces;

public interface IEmailTemplateService
{
    Task SendAsync(EmailType emailType, string email, EmailTemplateModel model, CancellationToken cancellationToken);
}