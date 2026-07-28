using FluentValidation;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeyById;

public class GetTranslationKeyByIdValidator : AbstractValidator<GetTranslationKeyByIdQuery>
{
    public GetTranslationKeyByIdValidator()
    {
        // Add validation rules here if needed
    }
}