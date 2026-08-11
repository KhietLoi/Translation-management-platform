using FluentValidation;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJob;

public class GetTranslationJobValidator : AbstractValidator<GetTranslationJobQuery>
{
    public GetTranslationJobValidator()
    {
        // Add validation rules here if needed
    }
}