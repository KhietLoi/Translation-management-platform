using FluentValidation;

namespace MySolution.Application.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Payload.Username)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}