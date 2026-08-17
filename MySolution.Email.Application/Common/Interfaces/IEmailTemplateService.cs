using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Common.Interfaces;

public interface IEmailTemplateService
{
    Task SendAsync <TModel> (
        EmailType emailType,
        string email,
        TModel model,
        CancellationToken cancellationToken =default)
        where TModel : class;
}