using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessImportTranslations;

public class ProcessImportTranslationsHandler : IRequestHandler<ProcessImportTranslationsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImportService _importService;
    private readonly ILogger<ProcessImportTranslationsHandler> _logger;
    
    public ProcessImportTranslationsHandler
    (
        IUnitOfWork unitOfWork,
        IImportService importService,
        ILogger<ProcessImportTranslationsHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _importService = importService;
        _logger = logger;
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
            
            if (string.IsNullOrWhiteSpace(job.BlobFileName))
            {
                throw new InvalidOperationException($"Import job {job.Id} missing BlobFileName.");
            }
            
            
            if (job.FileType == null)
            {
                throw new InvalidOperationException($"Import job {job.Id} missing FileType.");
            }
            
            var result = await _importService.ImportAsync(
                job.ProjectId,
                job.LanguageId.Value,
                job.NamespaceId.Value,
                job.BlobFileName!,
                job.FileType!.Value,
                cancellationToken);

            job.TotalRecords = result.TotalRecords;
            job.SuccessRecords = result.CreatedKeys + result.CreatedValues + result.UpdatedValues;
            job.SkippedRecords = result.SkippedRecords;
            job.FailedRecords = result.FailedRecords;
            job.Status = TranslationJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorMessage = null;
            
            await _unitOfWork.SaveAsync(cancellationToken);
            
            _logger.LogInformation("Import job {JobId} completed", job.Id);
        }
        catch (Exception e)
        {
            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = e.Message;
            job.FailedRecords = job.TotalRecords - job.SuccessRecords - job.SkippedRecords;
            job.CompletedAt = DateTime.UtcNow;
            
            await _unitOfWork.SaveAsync(cancellationToken);
            _logger.LogWarning("Import job {JobId} failed: {ErrorMessage}", job.Id, e.Message);
        }
    }
}