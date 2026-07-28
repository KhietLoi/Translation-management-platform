using FluentValidation;

namespace MySolution.Application.Features.TranslationKey.Commands.CreateTranslationKey;

public class CreateTranslationKeyValidator : AbstractValidator<CreateTranslationKeyCommand>
{
    public CreateTranslationKeyValidator()
    {
        RuleFor(x => x.Payload.ProjectId)
            .NotEmpty()
            .WithMessage("Please specify a project id.");
        RuleFor(x => x.Payload.Key)
            .NotEmpty()
            .WithMessage("Please specify a key.")
            .MaximumLength(200)
            .WithMessage("Key must not exceed 200 characters.");
        RuleFor(x => x.Payload.NamespaceId)
            .NotEmpty()
            .WithMessage("Please specify a namespaceId.");
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");
        
            
    }
}