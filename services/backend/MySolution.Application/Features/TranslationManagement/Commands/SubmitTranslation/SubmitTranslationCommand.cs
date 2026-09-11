using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.SubmitTranslation;

public class SubmitTranslationCommand : IRequest<SubmitTranslationResponse>
{
    public Guid Id { get; set; }
    public SubmitTranslationCommand(Guid id)
    {
        Id = id;
    }
}