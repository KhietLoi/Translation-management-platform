using FluentValidation;

namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Payload.CurrentPassword)
            .NotEmpty()
            .WithMessage("{PropertyName} can not be empty");

        RuleFor(x => x.Payload.NewPassword)
            .NotEmpty()
            .WithMessage("{PropertyName} can not be empty")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
            .WithMessage("{PropertyName} must contain at least 8 characters," +
                         " including an uppercase letter, a lowercase letter," +
                         " a digit, and a special character");
        RuleFor(x => x.Payload.NewPassword)
            .NotEqual(x => x.Payload.CurrentPassword)
            .WithMessage("{PropertyName} must be different from current password");
    }
}