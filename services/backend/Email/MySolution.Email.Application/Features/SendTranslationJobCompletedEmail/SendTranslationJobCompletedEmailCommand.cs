using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Application.Features.SendTranslationJobCompletedEmail;

public class SendTranslationJobCompletedEmailCommand : IRequest
{
    public TranslationJobCompletedEmailEvent Message { get; set; } = null!;
}