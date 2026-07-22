using FluentValidation;

namespace MySolution.Application.Features.User.Commands.CreateUser;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Payload.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(100)
            .WithMessage("Username cannot exceed 100 characters.");

        RuleFor(x => x.Payload.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email is invalid.");

        /*RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");*/

        RuleFor(x => x.Payload.RoleIds)
            .NotEmpty()
            .WithMessage("At least one role is required.");
    }
}