using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationKey;

public class CreateTranslationKeyCommand : IRequest<CreateTranslationKeyResponse>
{
    public CreateTranslationKeyRequest Payload { get; set; }

    public CreateTranslationKeyCommand(CreateTranslationKeyRequest payload)
    {
        Payload = payload;
    }
}