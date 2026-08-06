using MySolution.Email.Application.Common.Constants;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Common.Factories;

public class EmailTemplateFactory : IEmailTemplateFactory
{
    public EmailTemplateModel Create(string userName, string actionUrl, int expiryMinutes)
    {
        return new EmailTemplateModel
        {
            UserName = userName,
            ActionUrl = actionUrl,
            ExpiryMinutes = expiryMinutes,
            LogoUrl = EmailConstants.LogoUrl,
            Year = DateTime.UtcNow.Year
        };

    }
}