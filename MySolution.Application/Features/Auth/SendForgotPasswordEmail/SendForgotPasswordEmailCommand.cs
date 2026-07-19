using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.SendForgotPasswordEmail;

public class SendForgotPasswordEmailCommand : IRequest
{
    public SendForgotPasswordEmailEvent Message { get; set; } = default!;
}