using FluentValidation;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchUpdateTranslation;

public class BatchUpdateTranslationValidator : AbstractValidator<BatchUpdateTranslationCommand>
{
    public BatchUpdateTranslationValidator()
    {
        // Add validation rules here if needed
    }
}