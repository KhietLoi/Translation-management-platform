using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Application.Features.SendTranslationJobFailedEmail;

public class SendTranslationJobFailedEmailCommand : IRequest
{
    public TranslationJobFailedEmailEvent Message { get; set; }
    public SendTranslationJobFailedEmailCommand(TranslationJobFailedEmailEvent message)
    {
        Message = message;
    }
}