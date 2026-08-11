using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;

public class PublishTranslationsValidator : AbstractValidator<PublishTranslationsCommand>
{
    public PublishTranslationsValidator()
    {
        RuleFor(x => x.Payload.ProjectId)
            .NotEmpty()
            .WithMessage("Project id cannot be empty");
        
        RuleFor(x => x.Payload.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot be longer than 1000 characters");
    }
}