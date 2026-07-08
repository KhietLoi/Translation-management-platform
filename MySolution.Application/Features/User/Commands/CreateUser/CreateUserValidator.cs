using FluentValidation;

namespace MySolution.Application.Features.User.Commands.CreateUser;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Payload.Username)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Payload.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .MinimumLength(8);
        RuleFor(x => x.Payload.RoleIds)
            .NotEmpty()
            .WithMessage("At least one role is required.");
    }
}