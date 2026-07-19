using MediatR;
using MySolution.Application.Features.Auth.SendForgotPasswordEmail;
using MySolution.Application.Features.Auth.SendSetUpPasswordEmail;
using MySolution.Application.Features.Auth.SendVerifyEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Service;

public class MessageBusService : IMessageBusService
{
    private readonly IMediator _mediator;
    
    public MessageBusService (IMediator mediator)
    {
        _mediator = mediator;
    } 
    
    public async Task SendVerifyEmailAsync(SendVerifyEmailEvent message, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new SendVerifyEmailCommand { Message = message }, cancellationToken);
    }

    public async Task SendSetupPasswordEmailAsync(SendSetUpPasswordEmailEvent message, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new SendSetUpPasswordEmailCommand { Message = message }, cancellationToken);
    }

    public async Task SendForgotPasswordEmailAsync(SendForgotPasswordEmailEvent message, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new SendForgotPasswordEmailCommand { Message = message }, cancellationToken);
    }
}       