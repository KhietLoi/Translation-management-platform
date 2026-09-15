using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationValue;

public class CreateTranslationValueCommand : IRequest<CreateTranslationValueResponse>
{
    public CreateTranslationValueRequest Payload { get; set; }

    public CreateTranslationValueCommand(CreateTranslationValueRequest payload)
    {
        Payload = payload;
    }
}