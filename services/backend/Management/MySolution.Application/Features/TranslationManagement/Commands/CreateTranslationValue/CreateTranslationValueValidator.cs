using FluentValidation;

namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationValue;

public class CreateTranslationValueValidator : AbstractValidator<CreateTranslationValueCommand>
{
    public CreateTranslationValueValidator()
    {
        RuleFor(x => x.Payload.TranslationKeyId)
            .NotEmpty()
            .WithMessage("TranslationKeyId cannot be empty");
        RuleFor(x => x.Payload.LanguageId)
            .NotEmpty()
            .WithMessage("LanguageId cannot be empty");
        RuleFor(x => x.Payload.Value)
            .NotEmpty()
            .WithMessage("Value cannot be empty");
    }
}