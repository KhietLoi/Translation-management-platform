using FluentValidation;

namespace MySolution.Application.Features.ApiKey.Command.GenerateApiKey;

public class GenerateApiKeyValidator : AbstractValidator<GenerateApiKeyCommand>
{
    public GenerateApiKeyValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.Payload.NumofDaysExpires)
            .InclusiveBetween(0, 365)
            .WithMessage("Number of days until expiration must be between 0 and 365.");
    }
}