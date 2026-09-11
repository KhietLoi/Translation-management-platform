using FluentValidation;

namespace MySolution.Application.Features.Admin.User.Commands.ResendSetupPassword;

public class ResendSetupPasswordValidator : AbstractValidator<ResendSetupPasswordCommand>
{
    public ResendSetupPasswordValidator()
    {
        RuleFor(x => x.Payload.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid");
    }
}