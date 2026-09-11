using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ImportTranslations;

public class ImportTranslationsValidator : AbstractValidator<ImportTranslationsCommand>
{
    public ImportTranslationsValidator()
    {
        RuleFor(x => x.Payload.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required.");

        RuleFor(x => x.Payload.LanguageId)
            .NotEmpty().WithMessage("LanguageId is required.");

        RuleFor(x => x.Payload.Format)
            .IsInEnum().WithMessage("Invalid file format.");

        RuleFor(x => x.Payload.File)
            .NotNull().WithMessage("File is required.")
            .Must(file => file.Length > 0).WithMessage("File cannot be empty.");
    }
}