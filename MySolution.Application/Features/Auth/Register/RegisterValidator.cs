using FluentValidation;

namespace MySolution.Application.Features.Auth.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Payload.Username)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
        RuleFor(x => x.Payload.Email)
            .NotEmpty()
            .EmailAddress();
    }
}