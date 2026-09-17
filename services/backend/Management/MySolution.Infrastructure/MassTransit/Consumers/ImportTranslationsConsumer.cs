using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MySolution.Application.Common.Interfaces.DistributedLock;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.TranslationPipeline.Commands.ProcessImportTranslations;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class ImportTranslationsConsumer : IConsumer<ImportTranslationsEvent>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedLockService _distributedLockService;
    
    public ImportTranslationsConsumer
    (
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IDistributedLockService distributedLockService
    )
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _distributedLockService = distributedLockService;
    }
    
    public async Task Consume(ConsumeContext<ImportTranslationsEvent> context)
    {
        var job = await _unitOfWork.TranslationJob
            .GetAll()
            .FirstOrDefaultAsync(j => j.Id == context.Message.JobId, context.CancellationToken);
        if (job == null)
        {
            return;
        }

        var lockey = $"import-project: {job.ProjectId}";
        await using var lockHandle = await _distributedLockService.AcquireAsync(lockey, TimeSpan.FromMinutes(5));
        if (!lockHandle.IsAcquired)
        {
            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = "Project is currently being imported";
            await _unitOfWork.SaveAsync(context.CancellationToken);

            return;
        }
        await _mediator.Send(new ProcessImportTranslationsCommand( context.Message.JobId), context.CancellationToken);
    }
}