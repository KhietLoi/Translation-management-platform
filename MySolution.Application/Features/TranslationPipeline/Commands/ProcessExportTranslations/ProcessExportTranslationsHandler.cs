    using MediatR;
    using Microsoft.Extensions.Logging;
    using MySolution.Application.Common.Interfaces.File;
    using MySolution.Application.Common.Interfaces.MassTransit;
    using MySolution.Application.Common.Interfaces.Realtime;
    using MySolution.Application.Common.Interfaces.Repositories;
    using MySolution.Domain.Enums;
    using Shared.MassTransit.IntegrationEvents;

    namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessExportTranslations;

    public class ProcessExportTranslationsHandler
        : IRequestHandler<ProcessExportTranslationsCommand>
    {
        private readonly ILogger<ProcessExportTranslationsHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExportService _exportService;
        private readonly INotificationService _notificationService;
        private readonly IMessageSender _messageSender;

        public ProcessExportTranslationsHandler(
            ILogger<ProcessExportTranslationsHandler> logger,
            IUnitOfWork unitOfWork,
            IExportService exportService,
            INotificationService notificationService,
            IMessageSender messageSender)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _exportService = exportService;
            _notificationService = notificationService;
            _messageSender = messageSender;
        }

        public async Task Handle(
            ProcessExportTranslationsCommand request,
            CancellationToken cancellationToken)
        {
            var job = await _unitOfWork.TranslationJob
                .GetByIdAsync(request.Message.JobId);

            if (job == null)
            {
                _logger.LogError(
                    "Translation job {JobId} not found",
                    request.Message.JobId);

                return;
            }

            try
            {
                job.Status = TranslationJobStatus.Processing;
                job.StartedAt = DateTime.UtcNow;
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

                await _unitOfWork.SaveAsync(cancellationToken);
                
                var downloadUrl = result.DownloadUrl;
                var user = await _unitOfWork.User.GetByIdAsync(job.CreatedBy);
                await _messageSender.SendMessage<TranslationJobCompletedEmailEvent>(
                    new TranslationJobCompletedEmailEvent
                    {
                        JobId = job.Id,
                        UserId = job.CreatedBy,
                        Email = user!.Email,
                        ProjectId = job.ProjectId,
                        JobType = job.Type == TranslationJobType.Export ? "Export" : "Import",
                        FileName = job.FileName ?? string.Empty,
                        DownloadUrl = downloadUrl,
                        TotalRecords = job.TotalRecords,
                        SuccessRecords = job.SuccessRecords,
                        FailedRecords = job.FailedRecords,
                        SkippedRecords = job.SkippedRecords
                    }, cancellationToken);

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

                _logger.LogInformation("Export job {JobId} completed successfully", job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Export job {JobId} failed", job.Id);

                job.Status = TranslationJobStatus.Failed;
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveAsync(cancellationToken);
                
                await _notificationService.NotifyProjectAsync(
                    job.ProjectId,
                    job.CreatedBy,
                    "Export Failed",
                    ex.Message,
                    NotificationType.Error,
                    $"/translation-jobs/{job.Id}",
                    NotificationReferenceType.TranslationJob,
                    job.Id,
                    cancellationToken);
            }
        }
    }