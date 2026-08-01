using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.UpdateTranslationKey;

public class UpdateTranslationKeyCommand : IRequest<UpdateTranslationKeyResponse>
{
    public UpdateTranslationKeyRequest Payload { get; set; }
    public Guid TranslationKeyId { get; set; }

    public UpdateTranslationKeyCommand(UpdateTranslationKeyRequest payload, Guid translationKey)
    {
        Payload = payload;
        TranslationKeyId = translationKey;
    }
}