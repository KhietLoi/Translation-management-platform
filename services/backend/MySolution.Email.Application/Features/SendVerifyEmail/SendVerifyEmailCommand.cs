using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Application.Features.SendVerifyEmail;

public class SendVerifyEmailCommand : IRequest
{
    public SendVerifyEmailCommand()
    {
        Message = new SendVerifyEmailEvent();
    }

    public SendVerifyEmailEvent Message { get; set; }
}