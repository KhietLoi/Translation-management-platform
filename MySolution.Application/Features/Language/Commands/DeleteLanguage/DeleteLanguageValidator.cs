using FluentValidation;

namespace MySolution.Application.Features.Language.Commands.DeleteLanguage;

public class DeleteLanguageValidator : AbstractValidator<DeleteLanguageCommand>
{
    public DeleteLanguageValidator()
    {
        // Add validation rules here if needed
    }
}