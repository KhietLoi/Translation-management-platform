using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.DeleteTranslationValue;

public class DeleteTranslationValueCommand : IRequest<DeleteTranslationValueResponse>
{
    public Guid Id { get; set; }

    public DeleteTranslationValueCommand(Guid id)
    {
        Id = id;
    }
}