using MediatR;

namespace MySolution.Application.Features.Language.Commands.UpdateLanguage;

public class UpdateLanguageCommand : IRequest<UpdateLanguageResponse>
{
    public UpdateLanguageRequest Payload { get; set; }
    public Guid LanguageId { get; set; }
    public UpdateLanguageCommand(UpdateLanguageRequest payload,  Guid languageId)
    {
        Payload = payload;
        LanguageId = languageId;
    }
}