using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Email.Application.Features.SendVerifyEmail;
using Shared.MassTransit.Contracts;


namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendVerifyEmailConsumer : IConsumer<SendVerifyEmail>
{
    private const string EventName = "SendVerifyEmailConsumer";
    private readonly ILogger<SendVerifyEmailConsumer> _logger;
    private readonly IMediator _mediator;

    public SendVerifyEmailConsumer(IMediator mediator, ILogger<SendVerifyEmailConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendVerifyEmail> context)
    {
        var message = context.Message;
        _logger.LogInformation("Verify email message received. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);

        try
        {
            await _mediator.Send(new SendVerifyEmailCommand { Message = context.Message.Content }, context.CancellationToken);
            _logger.LogInformation("Verify email sent. EventName={EventName} , MessageId={MessageId}", EventName, context.MessageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while processing verify email message. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
    }
}