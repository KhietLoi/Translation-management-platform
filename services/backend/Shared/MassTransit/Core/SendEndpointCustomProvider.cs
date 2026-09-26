using MassTransit;
using Microsoft.Extensions.Logging;


namespace Shared.MassTransit.Core;

public class SendEndpointCustomProvider : ISendEndpointCustomProvider
{
    private readonly ILogger<SendEndpointCustomProvider> _logger;
    private readonly IBusControl _busControl;

    public SendEndpointCustomProvider(
        ILogger<SendEndpointCustomProvider> logger,
        IBusControl busControl)
    {
        _logger = logger;
        _busControl = busControl;
    }

    public ConnectHandle ConnectSendObserver(ISendObserver observer)
    {
        return _busControl.ConnectSendObserver(observer);
    }

    public Task<ISendEndpoint> GetSendEndpoint(Uri address)
    {
        return _busControl.GetSendEndpoint(address);
    }

    public async Task SendMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class
    {
        const string funcName = $"{nameof(SendEndpointCustomProvider)} {nameof(SendMessage)} =>";

        try
        {
            var queueName = QueueNameHelper.Get<T>();
            var endpoint = await GetSendEndpoint(new Uri($"queue:{queueName}"));
            var correlationId = Guid.NewGuid();
            await endpoint.Send<T>(
                new
                {
                    CorrelationId = correlationId,
                    Content = eventModel
                },
                cancellationToken);

            _logger.LogInformation(
                "{FunctionName} Message sent. Queue={QueueName}, MessageType={MessageType}, CorrelationId={CorrelationId}",
                funcName,
                queueName,
                typeof(T).Name,
                correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{FunctionName} Failed to send message. MessageType={MessageType}",
                funcName,
                typeof(T).Name);

            throw;
        }
    }
}