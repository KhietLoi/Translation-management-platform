using MediatR;

namespace MySolution.Application.Features.Language.Commands.UpdateLanguage;

public class UpdateLanguageCommand : IRequest<UpdateLanguageResponse>
{
    public UpdateLanguageRequest Payload { get; set; }

    public UpdateLanguageCommand(UpdateLanguageRequest payload)
    {
        Payload = payload;
    }
}