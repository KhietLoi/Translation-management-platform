using MediatR;
using Microsoft.Extensions.Logging;

using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessImportTranslations;

public class ProcessImportTranslationsHandler : IRequestHandler<ProcessImportTranslationsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImportService _importService;
    private readonly ILogger<ProcessImportTranslationsHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IMessageSender _messageSender;

    
    public ProcessImportTranslationsHandler
    (
        IUnitOfWork unitOfWork,
        IImportService importService,
        ILogger<ProcessImportTranslationsHandler> logger,
        INotificationService notificationService,
        IMessageSender messageSender
    )
    {
        _unitOfWork = unitOfWork;
        _importService = importService;
        _logger = logger;
        _notificationService = notificationService;
        _messageSender = messageSender;
    }
    
    public async Task Handle(ProcessImportTranslationsCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ProcessImportTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        
        var job = await _unitOfWork.TranslationJob.GetByIdAsync(request.JobId);
        if (job == null)
        {
            throw new Exception($"Job with id {request.JobId} not found");
        }
        
        try
        {
            job.Status =TranslationJobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync(cancellationToken);
            if (job.LanguageId == null)
            {
                throw new InvalidOperationException($"Import job {job.Id} missing LanguageId.");
            }

            if (job.NamespaceId == null)
            {
                throw new InvalidOperationException($"Import job {job.Id} missing NamespaceId.");
            }
            
            if (string.IsNullOrWhiteSpace(job.FileName))
            {
                throw new InvalidOperationException($"Import job {job.Id} missing FileName.");
            }
            
            var result = await _importService.ImportAsync
            (
                job.ProjectId,
                job.LanguageId.Value,
                job.NamespaceId.Value,
                job.FileName!,
                job.FileType,
                cancellationToken
            );

            job.TotalRecords = result.TotalRecords;
            job.SuccessRecords = result.CreatedValues+ result.UpdatedValues;
            job.SkippedRecords = result.SkippedRecords;
            job.FailedRecords = result.FailedRecords;
            job.Status = TranslationJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorMessage = null;
            
            await _unitOfWork.SaveAsync(cancellationToken);
                
                
            // Get User Email
            var user = await _unitOfWork.User.GetByIdAsync(job.CreatedBy);
            // Email:
            await _messageSender.SendMessage<TranslationJobCompletedEmailEvent>(
                new TranslationJobCompletedEmailEvent
                {
                    JobId = job.Id,
                    UserId = job.CreatedBy,
                    UserName = user!.Username,
                    ProjectName = job.Project.Name,
                    Email = user.Email,
                    ProjectId = job.ProjectId,
                    JobType = job.Type == TranslationJobType.Import ? "Import" : "Export",
                    FileName = job.FileName ?? string.Empty,
                    DownloadUrl = job.DownloadUrl!,
                    TotalRecords = job.TotalRecords,
                    SuccessRecords = job.SuccessRecords,
                    FailedRecords = job.FailedRecords,
                    SkippedRecords = job.SkippedRecords
                }, cancellationToken);
            _logger.LogInformation($" Import job {job.Id} completed and send email to {user.Email}");
                        
            await _notificationService.NotifyProjectAsync(
                job.ProjectId,
                job.CreatedBy,
                "Import Completed",
                $"File '{job.FileName}' imported successfully.",
                NotificationType.Success,
                $"/translation-jobs/{job.Id}",
                NotificationReferenceType.TranslationJob,
                job.Id,
                cancellationToken);
            _logger.LogInformation("Import job {JobId} completed", job.Id);
        }
        catch (Exception e)
        {
            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = e.Message;
            job.FailedRecords = job.TotalRecords - job.SuccessRecords - job.SkippedRecords;
            job.CompletedAt = DateTime.UtcNow;
            
            await _unitOfWork.SaveAsync(cancellationToken);
            await _notificationService.NotifyProjectAsync(
                job.ProjectId,
                job.CreatedBy,
                "Import Failed",
                e.Message,
                NotificationType.Error,
                $"/translation-jobs/{job.Id}",
                NotificationReferenceType.TranslationJob,
                job.Id,
                cancellationToken);
            _logger.LogWarning("Import job {JobId} failed: {ErrorMessage}", job.Id, e.Message);
        }
    }
}