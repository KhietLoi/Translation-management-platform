using MediatR;

namespace MySolution.Application.Features.Language.Commands.CreateLanguage;

public class CreateLanguageCommand : IRequest<CreateLanguageResponse>
{
    public CreateLanguageRequest Payload { get; set; }

    public CreateLanguageCommand(CreateLanguageRequest payload)
    {
        Payload = payload;
    }
}