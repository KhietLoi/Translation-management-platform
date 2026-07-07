using FluentValidation;

namespace MySolution.Application.Features.Auth.RefreshToken;

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