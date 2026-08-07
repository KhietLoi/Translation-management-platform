using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.ImportExport.Commands.ProcessExportTranslations;

public class ProcessExportTranslationsHandler : IRequestHandler<ProcessExportTranslationsCommand>
{
    private readonly ILogger<ProcessExportTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IExportService _exportService;

    public ProcessExportTranslationsHandler
    (
        ILogger<ProcessExportTranslationsHandler> logger,
		IUnitOfWork unitOfWork,
        IExportService exportService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _exportService = exportService;
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
        
        try
        {
            job.Status = TranslationJobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(cancellationToken);

            var result = await _exportService.ExportAsync(
                job.ProjectId,
                job.ExportFormat!.Value,
                cancellationToken);

            job.Status = TranslationJobStatus.Completed;
            job.FileName = result.FileName;
            job.DownloadUrl = result.DownloadUrl;
            job.CompletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export job {JobId} failed", job.Id);

            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }

    #endregion
}