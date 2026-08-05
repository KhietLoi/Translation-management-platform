using FluentValidation;

namespace MySolution.Application.Features.ApiKey.Command.GenerateApiKey;

public class GenerateApiKeyValidator : AbstractValidator<GenerateApiKeyCommand>
{
    public GenerateApiKeyValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Payload.ExpiresAt)
            .Must(x => x == null || x > DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future.");
    }
}