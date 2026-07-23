namespace MySolution.Email.Application.Common.Interfaces.MassTranssit;

public interface IMessageSender
{
    Task SendMessage<T>(object eventModel, CancellationToken cancellationToken = default) where T : class;
}