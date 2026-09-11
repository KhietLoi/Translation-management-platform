using FluentValidation;

namespace MySolution.Application.Features.Auth.Login;

/// <summary>
///     Validator for the LoginCommand
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
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
            .WithMessage(
                "Password must contain at least 8 characters," +
                " including an uppercase letter, a lowercase letter," +
                " a digit, and a special character");
    }
}