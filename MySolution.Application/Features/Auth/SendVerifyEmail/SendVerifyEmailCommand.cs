using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.SendVerifyEmail;

public class SendVerifyEmailCommand : IRequest
{
    public SendVerifyEmailCommand()
    {
        Message = new SendVerifyEmailEvent();
    }

    public SendVerifyEmailEvent Message { get; set; }
}