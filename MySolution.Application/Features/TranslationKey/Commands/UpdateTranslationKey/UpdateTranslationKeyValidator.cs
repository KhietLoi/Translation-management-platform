using FluentValidation;

namespace MySolution.Application.Features.TranslationKey.Commands.UpdateTranslationKey;

public class UpdateTranslationKeyValidator : AbstractValidator<UpdateTranslationKeyCommand>
{
    public UpdateTranslationKeyValidator()
    {
        RuleFor(x => x.Payload.Key)
            .NotEmpty()
            .WithMessage("Please specify a key.")
            .MaximumLength(200)
            .WithMessage("Key must not exceed 200 characters.");
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}