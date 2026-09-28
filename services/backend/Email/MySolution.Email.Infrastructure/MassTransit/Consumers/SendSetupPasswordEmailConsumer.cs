using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Email.Application.Features.SendSetUpPasswordEmail;
using Shared.MassTransit.Contracts;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendSetupPasswordEmailConsumer : IConsumer<SendSetUpPasswordEmail>
{
    private const string EventName = "SendSetupPasswordEmailConsumer";
    private readonly IMediator _mediator;
    private readonly ILogger<SendSetupPasswordEmailConsumer> _logger;

    public SendSetupPasswordEmailConsumer(IMediator mediator, ILogger<SendSetupPasswordEmailConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendSetUpPasswordEmail> context)
    {
        var message = context.Message;
        _logger.LogInformation("Setup password email message received. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);

        try
        {
            await _mediator.Send(new SendSetUpPasswordEmailCommand { Message = context.Message.Content }, context.CancellationToken);
            _logger.LogInformation("Setup password email sent. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while processing setup password email message. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
    }
}