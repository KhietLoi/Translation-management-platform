using MediatR;

namespace MySolution.Application.Features.TranslationValue.Commands.UpdateTranslationValue;

public class UpdateTranslationValueCommand : IRequest<UpdateTranslationValueResponse>
{
    public UpdateTranslationValueRequest Payload { get; set; }
    public Guid Id { get; set; }

    public UpdateTranslationValueCommand(UpdateTranslationValueRequest payload, Guid id)
    {
        Payload = payload;
        Id = id;
    }
}