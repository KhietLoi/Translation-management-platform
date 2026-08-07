using FluentValidation;

namespace MySolution.Application.Features.ImportExport.Commands.ExportTranslations;

public class ExportTranslationsValidator : AbstractValidator<ExportTranslationsCommand>
{
    public ExportTranslationsValidator()
    {
        RuleFor(x => x.Payload.ProjectId)
            .NotEqual(Guid.Empty)
            .WithMessage("Project ID is required.");
        RuleFor(x => x.Payload.ExportFormat)
            .IsInEnum();
    }
}