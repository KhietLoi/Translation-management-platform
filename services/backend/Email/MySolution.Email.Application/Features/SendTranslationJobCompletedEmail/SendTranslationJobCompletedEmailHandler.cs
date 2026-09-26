using MediatR;
using MySolution.Email.Application.Common.Bases;
using MySolution.Email.Application.Common.Enums;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Features.SendTranslationJobCompletedEmail;

public class SendTranslationJobCompletedEmailHandler : BaseEmailHandler,IRequestHandler<SendTranslationJobCompletedEmailCommand>
{
    private readonly IEmailTemplateFactory<TranslationJobCompletedEmailData, TranslationJobCompletedEmailModel> _emailTemplateFactory;

    public SendTranslationJobCompletedEmailHandler(
        IApplicationUrlProvider urlProvider,
        IEmailTemplateService emailTemplateService,
        IEmailTemplateFactory<TranslationJobCompletedEmailData, TranslationJobCompletedEmailModel> emailTemplateFactory)
        : base(urlProvider, emailTemplateService)
    {
        _emailTemplateFactory = emailTemplateFactory;
    }

    public Task Handle(SendTranslationJobCompletedEmailCommand request, CancellationToken cancellationToken)
    {
        var message = request.Message;

        var data = new TranslationJobCompletedEmailData
        {
            UserName = message.UserName,
            JobType = message.JobType,
            FileName = message.FileName,
            DownloadUrl = message.DownloadUrl,
            ProjectName = message.ProjectName,
            TotalRecords = message.TotalRecords,
            SuccessRecords = message.SuccessRecords,
            FailedRecords = message.FailedRecords,
            SkippedRecords = message.SkippedRecords
        };

        var model = _emailTemplateFactory.Create(data);
        return SendTemplateAsync(
            EmailType.TranslationJobCompleted,
            message.Email,
            model,
            cancellationToken);
    }
}