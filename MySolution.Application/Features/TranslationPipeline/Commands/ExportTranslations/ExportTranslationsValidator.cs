using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ExportTranslations;

public class ExportTranslationsValidator : AbstractValidator<ExportTranslationsCommand>
{
    public ExportTranslationsValidator()
    {
        RuleFor(x => x.Payload.ProjectId)
            .NotEqual(Guid.Empty)
            .WithMessage("Project ID is required.");
        RuleFor(x => x.Payload.Format)
            .IsInEnum();
    }
}