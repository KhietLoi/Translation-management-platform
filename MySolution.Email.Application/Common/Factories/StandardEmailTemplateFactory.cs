using MySolution.Email.Application.Common.Constants;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Common.Factories;

public class StandardEmailTemplateFactory : IEmailTemplateFactory <StandardEmailTemplateData, EmailTemplateModel>
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
    
    /*public TranslationJobCompletedEmailModel CreateTranslationJobCompleted(
        string userName,
        string jobType,
        string fileName,
        int totalRecords,
        int successRecords,
        int failedRecords,
        int skippedRecords)
    {
        return new TranslationJobCompletedEmailModel
        {
            UserName = userName,
            JobType = jobType,
            FileName = fileName,
            TotalRecords = totalRecords,
            SuccessRecords = successRecords,
            FailedRecords = failedRecords,
            SkippedRecords = skippedRecords,
            LogoUrl = EmailConstants.LogoUrl,
            Year = DateTime.UtcNow.Year
        };
    }

    public TranslationJobFailedEmailModel CreateTranslationJobFailed(
        string userName, 
        string jobType,
        string fileName,
        string errorMessage)
    {
        return new TranslationJobFailedEmailModel
        {
            UserName = userName,
            JobType = jobType,
            FileName = fileName,
            ErrorMessage = errorMessage,
            LogoUrl = EmailConstants.LogoUrl,
            Year = DateTime.UtcNow.Year
        };
    }*/
    public EmailTemplateModel Create(StandardEmailTemplateData input)
    {
        return new EmailTemplateModel
        {
            UserName = input.UserName,
            ActionUrl = input.ActionUrl,
            ExpiryMinutes = input.ExpiryMinutes,
            LogoUrl = EmailConstants.LogoUrl,
            Year = DateTime.UtcNow.Year
        };
    }
}
