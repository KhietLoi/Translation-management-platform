using FluentValidation;

namespace MySolution.Application.Features.TranslationKey.Queries.SearchTranslationKeys;

public class SearchTranslationKeysValidator : AbstractValidator<SearchTranslationKeysQuery>
{
    public SearchTranslationKeysValidator()
    {
        // Add validation rules here if needed
    }
}