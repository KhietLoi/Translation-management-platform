using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Service.MessageBus;

public interface IMessageBusService
{
    Task SendVerifyEmailAsync(SendVerifyEmailEvent message, CancellationToken cancellationToken = default);
    Task SendSetupPasswordEmailAsync(SendSetUpPasswordEmailEvent message, CancellationToken cancellationToken = default);
    Task SendForgotPasswordEmailAsync (SendForgotPasswordEmailEvent message, CancellationToken cancellationToken = default);
}