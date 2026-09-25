using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using Shared.MassTransit.Core;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessImportTranslations;

public class ProcessImportTranslationsHandler
    : IRequestHandler<ProcessImportTranslationsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImportService _importService;
    private readonly ILogger<ProcessImportTranslationsHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly ISendEndpointCustomProvider _messageSender;

    public ProcessImportTranslationsHandler(
        IUnitOfWork unitOfWork,
        IImportService importService,
        ILogger<ProcessImportTranslationsHandler> logger,
        INotificationService notificationService,
        ISendEndpointCustomProvider messageSender)
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

        var job = await _unitOfWork.TranslationJob
            .GetAll()
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        if (job == null)
        {
            _logger.LogWarning("{FunctionName} Job {JobId} not found", functionName, request.JobId);
            throw new Exception($"Job with id {request.JobId} not found");
        }

        try
        {
            if (job.LanguageId == null)
            {  
                _logger.LogWarning("{FunctionName} Job {JobId} not found", functionName, request.JobId);
                throw new InvalidOperationException($"Import job {job.Id} missing LanguageId.");
            }

            if (job.NamespaceId == null)
            {   
                _logger.LogWarning("{FunctionName} Job {JobId} not found", functionName, request.JobId);
                throw new InvalidOperationException($"Import job {job.Id} missing NamespaceId."); 
            }   

            if (string.IsNullOrWhiteSpace(job.FileName))
            {
                _logger.LogWarning("{FunctionName} Job {JobId} not found", functionName, request.JobId);
                throw new InvalidOperationException($"Import job {job.Id} missing FileName.");
            }

            job.Status = TranslationJobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;
            job.ErrorMessage = null;

            await _unitOfWork.SaveAsync(cancellationToken);

            _logger.LogInformation("Import job {JobId} is now Processing", job.Id);

            var result = await _importService.ImportAsync(
                job.ProjectId,
                job.LanguageId.Value,
                job.NamespaceId.Value,
                job.FileName,
                job.FileType,
                cancellationToken);

            job.TotalRecords = result.TotalRecords;
            job.SuccessRecords = result.CreatedValues + result.UpdatedValues;
            job.SkippedRecords = result.SkippedRecords;
            job.FailedRecords = result.FailedRecords;

            job.Status = TranslationJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorMessage = null;

            await _unitOfWork.SaveAsync(cancellationToken);

            _logger.LogInformation(
                "Import job {JobId} completed successfully. " +
                "Total: {TotalRecords}, " +
                "Success: {SuccessRecords}, " +
                "Skipped: {SkippedRecords}, " +
                "Failed: {FailedRecords}",
                job.Id,
                job.TotalRecords,
                job.SuccessRecords,
                job.SkippedRecords,
                job.FailedRecords);
        }
        catch (Exception ex)
        {
            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;

            job.FailedRecords = Math.Max(0, job.TotalRecords - job.SuccessRecords - job.SkippedRecords);

            try
            {
                await _unitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception saveException)
            {
                _logger.LogError(saveException, "Failed to save failed status for Import job {JobId}", job.Id);
            }

            _logger.LogError(ex, "Import job {JobId} failed: {ErrorMessage}", job.Id, ex.Message);
            await NotifyImportFailedAsync(job, ex.Message, cancellationToken);
            return;
        }

        await SendCompletionEmailAsync(job, cancellationToken);
        await SendCompletionNotificationAsync(job, cancellationToken);
        
        _logger.LogInformation("Import job {JobId} processing completed", job.Id);
    }

    private async Task SendCompletionEmailAsync(TranslationJob job, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _unitOfWork.User
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == job.CreatedBy, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Cannot send completion email for Job {JobId}. " + "User {UserId} was not found.", job.Id, job.CreatedBy);
                return;
            }

            await _messageSender.SendMessage<TranslationJobCompletedEmailEvent>(
                new TranslationJobCompletedEmailEvent
                {
                    JobId = job.Id,
                    UserId = job.CreatedBy,
                    UserName = user.Username,
                    ProjectName = job.Project.Name,
                    Email = user.Email,
                    ProjectId = job.ProjectId,
                    JobType = job.Type == TranslationJobType.Import
                        ? "Import"
                        : "Export",
                    FileName = job.FileName ?? string.Empty,
                    DownloadUrl = job.DownloadUrl ?? string.Empty,
                    TotalRecords = job.TotalRecords,
                    SuccessRecords = job.SuccessRecords,
                    FailedRecords = job.FailedRecords,
                    SkippedRecords = job.SkippedRecords
                }, cancellationToken);

            _logger.LogInformation("Completion email event sent for Import job {JobId} to {Email}", job.Id, user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send completion email for Import job {JobId}", job.Id);
        }
    }

    private async Task SendCompletionNotificationAsync(TranslationJob job, CancellationToken cancellationToken)
    {
        try
        {
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

            _logger.LogInformation("Completion notification sent for Import job {JobId}", job.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send completion notification for Import job {JobId}", job.Id);
        }
    }

    private async Task NotifyImportFailedAsync(TranslationJob job, string errorMessage, CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.NotifyProjectAsync(
                job.ProjectId,
                job.CreatedBy,
                "Import Failed",
                errorMessage,
                NotificationType.Error,
                $"/translation-jobs/{job.Id}",
                NotificationReferenceType.TranslationJob,
                job.Id,
                cancellationToken);

            _logger.LogInformation("Failure notification sent for Import job {JobId}", job.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send failure notification for Import job {JobId}", job.Id);
        }
    }
}