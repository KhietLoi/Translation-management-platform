using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.SendSetUpPasswordEmail;

public class SendSetUpPasswordEmailCommand : IRequest
{
    public SendSetUpPasswordEmailEvent Message { get; set; } = default!;
}