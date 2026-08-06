using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Common.Interfaces;

public interface IEmailTemplateFactory
{
    EmailTemplateModel Create(string userName, string actionUrl, int expiryMinutes);
}