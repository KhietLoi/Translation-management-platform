using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Commands.RollbackRelease;

public class RollbackReleaseValidator : AbstractValidator<RollbackReleaseCommand>
{
    public RollbackReleaseValidator()
    {
        RuleFor(x => x.ReleaseId)
            .NotEmpty()
            .WithMessage("{PropertyName} can not be empty");
    }
}