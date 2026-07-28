using FluentValidation;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeys;

public class GetTranslationKeysValidator : AbstractValidator<GetTranslationKeysQuery>
{
    public GetTranslationKeysValidator()
    {
        // Add validation rules here if needed
    }
}