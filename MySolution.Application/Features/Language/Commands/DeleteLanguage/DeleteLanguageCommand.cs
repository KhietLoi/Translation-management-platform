using MediatR;

namespace MySolution.Application.Features.Language.Commands.DeleteLanguage;

public class DeleteLanguageCommand : IRequest<DeleteLanguageResponse>
{
    public DeleteLanguageRequest Payload { get; set; }

    public DeleteLanguageCommand(DeleteLanguageRequest payload)
    {
        Payload = payload;
    }
}