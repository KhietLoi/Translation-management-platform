using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDiff;

public class GetReleaseDiffValidator : AbstractValidator<GetReleaseDiffQuery>
{
    public GetReleaseDiffValidator()
    {
        RuleFor(x => x.SourceReleaseId)
            .NotEmpty()
            .WithMessage("The source release id cannot be empty");
    }
}