using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDiff;

public class GetReleaseDiffValidator : AbstractValidator<GetReleaseDiffQuery>
{
    public GetReleaseDiffValidator()
    {
        RuleFor(x => x.TargetReleaseId)
            .NotEmpty()
            .WithMessage("The target release id cannot be empty");
    }
}