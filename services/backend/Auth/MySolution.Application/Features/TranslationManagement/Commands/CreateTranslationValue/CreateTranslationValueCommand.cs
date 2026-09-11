using MediatR;

namespace MySolution.Application.Features.TranslationValue.Commands.CreateTranslationValue;

public class CreateTranslationValueCommand : IRequest<CreateTranslationValueResponse>
{
    public CreateTranslationValueRequest Payload { get; set; }

    public CreateTranslationValueCommand(CreateTranslationValueRequest payload)
    {
        Payload = payload;
    }
}