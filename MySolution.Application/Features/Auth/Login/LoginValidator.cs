using FluentValidation;

namespace MySolution.Application.Features.Auth.Login;
/// <summary>
/// Validator for the LoginCommand
/// </summary>
public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Payload.Username)
            .NotEmpty()
            .WithMessage("Username is required!")
            .MaximumLength(100);
        RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 8 characters long!");
    }
}