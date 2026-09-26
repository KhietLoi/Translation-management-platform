    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MySolution.Application.Common.Interfaces.File;
    using MySolution.Application.Common.Interfaces.Realtime;
    using MySolution.Application.Common.Interfaces.Repositories;
    using MySolution.Domain.Entities;
    using MySolution.Domain.Enums;
    using Shared.MassTransit.Contracts;
    using Shared.MassTransit.Core;
    using Shared.MassTransit.IntegrationEvents;

    namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessExportTranslations;

    public class ProcessExportTranslationsHandler : IRequestHandler<ProcessExportTranslationsCommand>
    {   
        private readonly ILogger<ProcessExportTranslationsHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExportService _exportService;
        private readonly INotificationService _notificationService;
        private readonly ISendEndpointCustomProvider _messageSender;

        public ProcessExportTranslationsHandler(
            ILogger<ProcessExportTranslationsHandler> logger,
            IUnitOfWork unitOfWork,
            IExportService exportService,
            INotificationService notificationService,
            ISendEndpointCustomProvider messageSender)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _exportService = exportService;
            _notificationService = notificationService;
            _messageSender = messageSender;
        }

        public async Task Handle(ProcessExportTranslationsCommand request, CancellationToken cancellationToken)
        {
            var functionName = $"{nameof(ProcessExportTranslationsHandler)} =>";
            _logger.LogInformation(functionName);  
            
            var jobId = request.Message.JobId;
            var job = await _unitOfWork.TranslationJob
                .GetAll()
                .Include(j => j.Project)
                .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
            if (job == null)
            {
                _logger.LogError("Translation job {JobId} not found", jobId);
                return;
            }

            try
            {
                job.Status = TranslationJobStatus.Processing;
                job.StartedAt = DateTime.UtcNow;
                job.ErrorMessage = null;
                await _unitOfWork.SaveAsync(cancellationToken);

                var result = await _exportService.ExportAsync(job.ProjectId, job.FileType, cancellationToken);

                job.Status = TranslationJobStatus.Completed;
                job.FileName = result.FileName;
                job.DownloadUrl = result.DownloadUrl;
                job.CompletedAt = DateTime.UtcNow;
                job.TotalRecords = result.TotalRecords;
                job.SuccessRecords = result.SuccessRecords;
                job.FailedRecords = result.FailedRecords;
                job.SkippedRecords = result.SkippedRecords;
                job.ErrorMessage = null;

                await _unitOfWork.SaveAsync(cancellationToken);
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
                    _logger.LogError(saveException, "Failed to save failed status for Export job {JobId}", job.Id);
                }

                _logger.LogError(ex, "Export job {JobId} failed: {ErrorMessage}", job.Id, ex.Message);

                await NotifyExportFailedAsync(job, ex.Message, cancellationToken);
                return;
            }

            await SendCompletionEmailAsync(job, cancellationToken);
            await SendCompletionNotificationAsync(job, cancellationToken);

            _logger.LogInformation("Export job {JobId} processing completed", job.Id);
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
                    return;
                }
                
                var translationJobCompletedEmailEvent = new TranslationJobCompletedEmailEvent
                {
                    JobId = job.Id,
                    UserId = job.CreatedBy,
                    UserName = user.Username,
                    ProjectName = job.Project.Name,
                    Email = user.Email,
                    ProjectId = job.ProjectId,
                    JobType = job.Type == TranslationJobType.Export ? "Export" : "Import",
                    FileName = job.FileName ?? string.Empty,
                    DownloadUrl = job.DownloadUrl ?? string.Empty,
                    TotalRecords = job.TotalRecords,
                    SuccessRecords = job.SuccessRecords,
                    FailedRecords = job.FailedRecords,
                    SkippedRecords = job.SkippedRecords
                };
                
                await _messageSender.SendMessage<TranslationJobCompletedEmail>(translationJobCompletedEmailEvent, cancellationToken);
                _logger.LogInformation("Completion email event sent for Export job {JobId} to {Email}", job.Id, user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send completion email for Export job {JobId}", job.Id);
            }
        }

        private async Task SendCompletionNotificationAsync(TranslationJob job, CancellationToken cancellationToken)
        {
            try
            {
                await _notificationService.NotifyProjectAsync(
                    job.ProjectId,
                    job.CreatedBy,
                    "Export Completed",
                    $"Export file '{job.FileName}' completed successfully.",
                    NotificationType.Success,
                    $"/translation-jobs/{job.Id}",
                    NotificationReferenceType.TranslationJob,
                    job.Id,
                    cancellationToken);

                _logger.LogInformation("Completion notification sent for Export job {JobId}", job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send completion notification for Export job {JobId}", job.Id);
            }
        }

        private async Task NotifyExportFailedAsync(TranslationJob job, string errorMessage, CancellationToken cancellationToken)
        {
            try
            {
                await _notificationService.NotifyProjectAsync(
                    job.ProjectId,
                    job.CreatedBy,
                    "Export Failed",
                    errorMessage,
                    NotificationType.Error,
                    $"/translation-jobs/{job.Id}",
                    NotificationReferenceType.TranslationJob,
                    job.Id,
                    cancellationToken);

                _logger.LogInformation("Failure notification sent for Export job {JobId}", job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send failure notification for Export job {JobId}", job.Id);
            }
        }
    }
