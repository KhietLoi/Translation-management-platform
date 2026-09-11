using MediatR;

namespace MySolution.Application.Features.Language.Commands.DeleteLanguage;

public class DeleteLanguageCommand : IRequest<DeleteLanguageResponse>
{
    public Guid LanguageId { get; set; }

    public DeleteLanguageCommand(Guid languageId)
    {
        LanguageId = languageId;
    }
}