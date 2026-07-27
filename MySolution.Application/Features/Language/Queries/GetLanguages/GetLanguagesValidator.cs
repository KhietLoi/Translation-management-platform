using FluentValidation;

namespace MySolution.Application.Features.Language.Queries.GetLanguages;

public class GetLanguagesValidator : AbstractValidator<GetLanguagesQuery>
{
    public GetLanguagesValidator()
    {
        // Add validation rules here if needed
    }
}