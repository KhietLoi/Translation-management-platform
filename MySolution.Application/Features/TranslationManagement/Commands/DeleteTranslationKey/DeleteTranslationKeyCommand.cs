using MediatR;
using MySolution.Application.Features.TranslationManagement.Commands.DeleteTranslationKey;

namespace MySolution.Application.Features.TranslationKey.Commands.DeleteTranslationKey;

public class DeleteTranslationKeyCommand : IRequest<DeleteTranslationKeyResponse>
{
    public Guid Id { get; set; }

    public DeleteTranslationKeyCommand(Guid id)
    {
        Id = id;
    }
}