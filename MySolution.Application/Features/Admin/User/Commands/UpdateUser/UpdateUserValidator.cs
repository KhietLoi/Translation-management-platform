using FluentValidation;

namespace MySolution.Application.Features.Admin.User.Commands.UpdateUser;

public class UpdateUserValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
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

        RuleFor(x => x.Payload.RoleIds)
            .NotEmpty()
            .WithMessage("At least one role is required.");
    }
}