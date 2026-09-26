using MassTransit;

namespace Shared.MassTransit.Core;

public interface ISendEndpointCustomProvider : ISendEndpointProvider
{
    Task SendMessage<T>(object eventModel, CancellationToken cancellationToken) where T : class;
}