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
            .MinimumLength(6)
            .WithMessage("{PropertyName} must contain at least 6 characters");

        RuleFor(x => x.Payload.ConfirmNewPassword)
            .Equal(x => x.Payload.NewPassword)
            .WithMessage("Passwords do not match");
    }
}