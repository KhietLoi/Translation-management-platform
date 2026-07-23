using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Application.Features.SendForgotPasswordEmail;

public class SendForgotPasswordEmailCommand : IRequest
{
    public SendForgotPasswordEmailEvent Message { get; set; } = default!;
}