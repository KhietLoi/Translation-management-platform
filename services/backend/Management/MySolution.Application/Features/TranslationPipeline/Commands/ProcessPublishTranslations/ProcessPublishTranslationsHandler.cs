    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MySolution.Application.Common.Interfaces.File;
    using MySolution.Application.Common.Interfaces.Realtime;
    using MySolution.Application.Common.Interfaces.Repositories;
    using MySolution.Domain.Enums;

    namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessPublishTranslations;

    public class ProcessPublishTranslationsHandler
        : IRequestHandler<ProcessPublishTranslationsCommand>
    {
        private readonly ILogger<ProcessPublishTranslationsHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishService _publishService;
        private readonly INotificationService _notificationService;

        public ProcessPublishTranslationsHandler
        (
            ILogger<ProcessPublishTranslationsHandler> logger,
            IUnitOfWork unitOfWork,
            IPublishService publishService,
            INotificationService notificationService
        )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _publishService = publishService;
            _notificationService =  notificationService;
        }

        public async Task Handle(ProcessPublishTranslationsCommand request, CancellationToken cancellationToken)
        {
            var functionName = $"{nameof(ProcessPublishTranslationsHandler)} =>";
            _logger.LogInformation("{FunctionName} Start processing publish job {JobId}", functionName, request.JobId);

            var job = await _unitOfWork.TranslationJob
                .GetAll()
                .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
            if (job == null)
            {
                throw new Exception($"{nameof(ProcessPublishTranslationsHandler)} job with id {request.JobId} not found");
            }
            
            try
            {
                job.Status = TranslationJobStatus.Processing;
                job.StartedAt = DateTime.UtcNow;
                
                await _unitOfWork.SaveAsync(cancellationToken);
                _logger.LogInformation($"CreatedBy: {job.CreatedBy}");
                var result = await _publishService.PublishAsync(job.Id, job.ProjectId, job.CreatedBy, job.Notes, cancellationToken);

                job.TotalRecords = result.TotalRecords;
                job.SuccessRecords = result.SuccessRecords;
                job.SkippedRecords = result.SkippedRecords;
                job.FailedRecords = result.FailedRecords;

                job.FileName = result.BlobFileName;
                job.DownloadUrl = result.DownloadUrl;

                job.Status = TranslationJobStatus.Completed;
                job.CompletedAt = DateTime.UtcNow;
                job.ErrorMessage = null;

                await _unitOfWork.SaveAsync(cancellationToken);
                await _notificationService.NotifyProjectAsync(
                    job.ProjectId,
                    job.CreatedBy,
                    "Publish Completed",
                    "Translations published successfully.",
                    NotificationType.Success,
                    $"/projects/{job.ProjectId}/releases",
                    NotificationReferenceType.TranslationRelease,
                    result.ReleaseId,
                    cancellationToken);
                _logger.LogInformation("{FunctionName} Publish job {JobId} completed successfully", functionName, request.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{FunctionName} Publish job {JobId} failed", functionName, request.JobId);
                job.Status = TranslationJobStatus.Failed;
                job.ErrorMessage = ex.Message;
                job.CompletedAt = DateTime.UtcNow;
                job.FailedRecords = job.TotalRecords -job.SuccessRecords - job.SkippedRecords;
                await _unitOfWork.SaveAsync(cancellationToken);
                await _notificationService.NotifyProjectAsync(
                    job.ProjectId,
                    job.CreatedBy,
                    "Publish Failed",
                    ex.Message,
                    NotificationType.Error,
                    $"/projects/{job.ProjectId}/releases",
                    null,
                    null,
                    cancellationToken);
                throw;
            }
        }
    }