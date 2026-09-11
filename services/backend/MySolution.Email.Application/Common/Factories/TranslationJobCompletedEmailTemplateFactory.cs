using MySolution.Email.Application.Common.Constants;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Common.Factories;

public class TranslationJobCompletedEmailTemplateFactory
    : IEmailTemplateFactory<
        TranslationJobCompletedEmailData,
        TranslationJobCompletedEmailModel>
{
    public TranslationJobCompletedEmailModel Create(
        TranslationJobCompletedEmailData data)
    {
        return new TranslationJobCompletedEmailModel
        {
            UserName = data.UserName,
            JobType = data.JobType,
            FileName = data.FileName,
            DownloadUrl = data.DownloadUrl,
            ProjectName = data.ProjectName,

            TotalRecords = data.TotalRecords,
            SuccessRecords = data.SuccessRecords,
            FailedRecords = data.FailedRecords,
            SkippedRecords = data.SkippedRecords,

            LogoUrl = EmailConstants.LogoUrl,
            Year = DateTime.UtcNow.Year
        };
    }
}