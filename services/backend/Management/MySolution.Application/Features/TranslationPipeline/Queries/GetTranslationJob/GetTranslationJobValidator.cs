using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJob;

public class GetTranslationJobValidator : AbstractValidator<GetTranslationJobQuery>
{
    public GetTranslationJobValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Please specify a project ID");
    }
}