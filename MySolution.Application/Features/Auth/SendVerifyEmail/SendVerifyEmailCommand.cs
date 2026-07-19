using System.Net.Mail;
using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.SendVerifyEmail;

public class SendVerifyEmailCommand : IRequest
{
    public SendVerifyEmailEvent Message { get; set; }

    public SendVerifyEmailCommand()
    {
        Message = new SendVerifyEmailEvent();
    }

}