using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.DeleteTranslationKey;

public class DeleteTranslationKeyCommand : IRequest<DeleteTranslationKeyResponse>
{
    public Guid Id { get; set; }

    public DeleteTranslationKeyCommand(Guid id)
    {
        Id = id;
    }
}