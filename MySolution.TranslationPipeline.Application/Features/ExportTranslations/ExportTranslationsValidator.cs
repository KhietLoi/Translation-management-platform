using FluentValidation;

namespace MySolution.TranslationPipeline.Application.Features.ExportTranslations;

public class ExportTranslationsValidator : AbstractValidator<ExportTranslationsCommand>
{
    public ExportTranslationsValidator()
    {
        // Add validation rules here if needed
    }
}