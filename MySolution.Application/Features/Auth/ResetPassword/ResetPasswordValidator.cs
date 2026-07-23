using FluentValidation;

namespace MySolution.Application.Features.Auth.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Payload.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
            .WithMessage(
                "New password must contain at least 8 characters, including an uppercase letter, a lowercase letter, a digit, and a special character");
    }
}