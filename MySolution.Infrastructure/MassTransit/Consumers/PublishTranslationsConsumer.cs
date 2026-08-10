using MassTransit;
using MediatR;
using MySolution.Application.Features.Publish.Commands.ProcessPublishTranslations;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class PublishTranslationsConsumer : IConsumer<PublishTranslationsEvent>
{
    private readonly IMediator _mediator;

    public PublishTranslationsConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<PublishTranslationsEvent> context)
    {
        await _mediator.Send (new ProcessPublishTranslationsCommand(context.Message.JobId), context.CancellationToken);
    }
}