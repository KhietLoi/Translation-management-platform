using MassTransit;
using MediatR;
using MySolution.Application.Common.Interfaces.DistributedLock;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.Publish.Commands.ProcessPublishTranslations;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class PublishTranslationsConsumer : IConsumer<PublishTranslationsEvent>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork  _unitOfWork;
    private readonly IDistributedLockService _distributedLockService;

    public PublishTranslationsConsumer
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
    
    public async Task Consume(ConsumeContext<PublishTranslationsEvent> context)
    {
        var job = await _unitOfWork.TranslationJob.GetByIdAsync(context.Message.JobId);
        if (job == null)
        {
            return;
        }

        var lockey = $"publish-project:{job.ProjectId}";
        await using var lockHandle =  await _distributedLockService.AcquireAsync(lockey,
            TimeSpan.FromMinutes(5), context.CancellationToken);

        if (!lockHandle.IsAcquired)
        {
            job.Status = TranslationJobStatus.Failed;
            job.ErrorMessage = "Project is currently being published";
            await _unitOfWork.SaveAsync(context.CancellationToken);

            return;
        }
        
        await _mediator.Send (new ProcessPublishTranslationsCommand(context.Message.JobId), context.CancellationToken);
    }
}