using FluentValidation;

namespace MySolution.Application.Features.Auth.Register;

/// <summary>
///     Validator for the RegisterCommand
/// </summary>
public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Payload.Username)
            .NotEmpty()
            .WithMessage("Username is required!")
            .MaximumLength(100)
            .WithMessage("Username must be less than 100 characters!");

        RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .WithMessage("Password is required!")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
            .WithMessage("Password must contain at least 8 characters," +
                         " including an uppercase letter, a lowercase letter," +
                         " a digit, and a special character");

        RuleFor(x => x.Payload.Email)
            .NotEmpty()
            .WithMessage("Email is required!")
            .EmailAddress()
            .WithMessage("Please specify a valid email address!");
    }
}