using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Email.Application.Features.SendForgotPasswordEmail;
using Shared.MassTransit.Contracts;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendForgotPasswordEmailConsumer : IConsumer<SendForgotPasswordEmail>
{
    private const string EventName = "SendForgotPasswordEmailConsumer";
    private readonly IMediator _mediator;
    private readonly ILogger<SendForgotPasswordEmailConsumer> _logger;

    public SendForgotPasswordEmailConsumer(IMediator mediator, ILogger<SendForgotPasswordEmailConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendForgotPasswordEmail> context)
    {
        var message = context.Message;
        _logger.LogInformation("Forgot password email message received. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);

        try
        {
            await _mediator.Send(new SendForgotPasswordEmailCommand (context.Message.Content), context.CancellationToken);
            _logger.LogInformation("Forgot password email sent. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while processing forgot password email message. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
    }
}