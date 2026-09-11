using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.DistributedLock;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.TranslationPipeline.Commands.ProcessPublishTranslations;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class PublishTranslationsConsumer : IConsumer<PublishTranslationsEvent>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork  _unitOfWork;
    private readonly IDistributedLockService _distributedLockService;
    public readonly ILogger<PublishTranslationsConsumer> _logger;

    public PublishTranslationsConsumer
    (
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IDistributedLockService distributedLockService,
        ILogger<PublishTranslationsConsumer> logger
    )
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _distributedLockService = distributedLockService;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<PublishTranslationsEvent> context)
    {
        var job = await _unitOfWork.TranslationJob.GetByIdAsync(context.Message.JobId);
        if (job == null)
        {
            return;
        }

        var lockey = $"publish-project:{job.ProjectId}";
        
        _logger.LogWarning(
            "[REDLOCK] Trying to acquire lock. Project={ProjectId}, Job={JobId}",
            job.ProjectId,
            job.Id);
        await using var lockHandle =  await _distributedLockService.AcquireAsync(lockey,
            TimeSpan.FromMinutes(5), context.CancellationToken);

        if (!lockHandle.IsAcquired)
        {
            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = "Project is currently being published";
            await _unitOfWork.SaveAsync(context.CancellationToken);
            return;
        }

        _logger.LogWarning(
            "[REDLOCK] Acquired={Acquired}. Project={ProjectId}, Job={JobId}",
            lockHandle.IsAcquired,
            job.ProjectId,
            job.Id);
        
        await _mediator.Send (new ProcessPublishTranslationsCommand(context.Message.JobId), context.CancellationToken);
    }
}