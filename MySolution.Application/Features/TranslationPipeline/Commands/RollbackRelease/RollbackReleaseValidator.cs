using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Commands.RollbackRelease;

public class RollbackReleaseValidator : AbstractValidator<RollbackReleaseCommand>
{
    public RollbackReleaseValidator()
    {
        // Add validation rules here if needed
    }
}