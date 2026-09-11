using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.RejectTranslation;

public class RejectTranslationCommand : IRequest<RejectTranslationResponse>
{
    public RejectTranslationRequest Payload { get; set; }
    public Guid Id { get; set; }

    public RejectTranslationCommand(RejectTranslationRequest payload,  Guid id)
    {
        Payload = payload;
        Id = id;
    }
}