using FluentValidation;

namespace MySolution.Application.Features.TranslationValue.Commands.UpdateTranslationValue;

public class UpdateTranslationValueValidator : AbstractValidator<UpdateTranslationValueCommand>
{
    public UpdateTranslationValueValidator()
    {
        RuleFor(x => x.Payload.Value)
            .NotEmpty()
            .WithMessage("Payload value cannot be empty");
    }
}