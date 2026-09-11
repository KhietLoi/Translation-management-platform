namespace MySolution.Application.Common.Interfaces.MassTransit;

public interface IMessageSender
{
    Task SendMessage<T>(object eventModel, CancellationToken cancellationToken = default) where T : class;
}