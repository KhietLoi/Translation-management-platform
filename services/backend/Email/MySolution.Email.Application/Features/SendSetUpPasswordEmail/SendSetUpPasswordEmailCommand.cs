using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Application.Features.SendSetUpPasswordEmail;

public class SendSetUpPasswordEmailCommand : IRequest
{
    public SendSetUpPasswordEmailEvent Message { get; set; } = default!;
}