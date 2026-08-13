using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessExportTranslations;

public class ProcessExportTranslationsHandler : IRequestHandler<ProcessExportTranslationsCommand>
{
    private readonly ILogger<ProcessExportTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IExportService _exportService;
    private readonly INotificationService _notificationService;

    public ProcessExportTranslationsHandler
    (
        ILogger<ProcessExportTranslationsHandler> logger,
		IUnitOfWork unitOfWork,
        IExportService exportService,
        INotificationService notificationService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _exportService = exportService;
        _notificationService = notificationService;
    }

    #region Implementation of IRequestHandler<in ProcessExportTranslationsCommand, ProcessExportTranslationsResponse>

    public async Task Handle(ProcessExportTranslationsCommand request, CancellationToken cancellationToken)
    {
        var job = await _unitOfWork.TranslationJob.GetByIdAsync(request.Message.JobId);
        if (job == null)
        {
            _logger.LogError($"Job with id {request.Message.JobId} not found");
            return;
        }
        var username = await _unitOfWork.User.GetUserNameAsync(job.CreatedBy);
        try
        {
            job.Status = TranslationJobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(cancellationToken);

            var result = await _exportService.ExportAsync(
                job.ProjectId,
                job.FileType,
                cancellationToken);

            job.Status = TranslationJobStatus.Completed;
            job.FileName = result.FileName;
            job.DownloadUrl = result.DownloadUrl;
            job.CompletedAt = DateTime.UtcNow;
            job.TotalRecords = result.TotalRecords;
            job.SkippedRecords = result.SkippedRecords;
            job.FailedRecords = result.FailedRecords;
            job.SuccessRecords  = result.SuccessRecords;

            await _unitOfWork.SaveAsync(cancellationToken);
          
            await _notificationService.NotifyProjectAsync(
                job.ProjectId,
                "Export Completed",
                $"Export file '{job.FileName}' completed successfully.",
                username,
                NotificationType.Success,
                $"/translation-jobs/{job.Id}",
                cancellationToken);
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
                    "Export Failed",
                    ex.Message, 
                    username,
                NotificationType.Error,
                $"/translation-jobs/{job.Id}",
                cancellationToken);
        }
    }

    #endregion
}