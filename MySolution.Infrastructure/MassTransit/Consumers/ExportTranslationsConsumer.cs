using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.ImportExport.Commands.ProcessExportTranslations;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class ExportTranslationsConsumer
    : IConsumer<ExportTranslationsEvent>
{
    private readonly IMediator _mediator;

    public ExportTranslationsConsumer(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ExportTranslationsEvent> context)
    {
        await _mediator.Send(new ProcessExportTranslationsCommand{ Message = context.Message }, context.CancellationToken);
        /*var message = context.Message;

        var job = await _unitOfWork.TranslationJob
            .GetByIdAsync(message.JobId);

        if (job == null)
        {
            _logger.LogWarning(
                "Export job {JobId} not found",
                message.JobId);

            return;
        }

        try
        {
            job.Status = TranslationJobStatus.Processing;
            job.StartedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(context.CancellationToken);

            var result = await _exportService.ExportAsync(
                job.ProjectId,
                job.ExportFormat!.Value,
                context.CancellationToken);

            job.Status = TranslationJobStatus.Completed;
            job.FileName = result.FileName;
            job.DownloadUrl = result.DownloadUrl;
            job.CompletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Export job {JobId} failed",
                job.Id);

            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(context.CancellationToken);
        }*/
    }

  
}