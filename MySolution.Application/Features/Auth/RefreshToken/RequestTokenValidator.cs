using FluentValidation;

namespace MySolution.Application.Features.Auth.RefreshToken;

/// <summary>
/// Validator for the RefreshTokenCommand, ensuring that the required fields are not empty.
/// </summary>
public class RequestTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RequestTokenValidator()
    {
        RuleFor(x => x.Payload.AccessToken)
            .NotEmpty();

        RuleFor(x => x.Payload.RefreshToken)
            .NotEmpty();
    }
}