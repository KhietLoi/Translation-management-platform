using MassTransit;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.MassTransit;

namespace MySolution.Infrastructure.MassTransit;

public class SendEndPointCustomProvider : IMessageSender
{
    private readonly IBusControl _busControl;
    private readonly ILogger<SendEndPointCustomProvider> _logger;

    public SendEndPointCustomProvider(IBusControl busControl, ILogger<SendEndPointCustomProvider> logger)
    {
        _busControl = busControl;
        _logger = logger;
    }

    public async Task SendMessage<T>(object eventModel, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var formatter = new KebabCaseEndpointNameFormatter(false);
            var queueName = formatter.SanitizeName(typeof(T).Name);
            _logger.LogInformation("Sending message to queue: {QueueName}", queueName);
            var endpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send<T>(eventModel, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending message");
            throw;
        }
    }
}