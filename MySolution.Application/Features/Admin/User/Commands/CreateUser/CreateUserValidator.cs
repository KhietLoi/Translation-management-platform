using FluentValidation;

namespace MySolution.Application.Features.Admin.User.Commands.CreateUser;

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
            .Matches(@"^(?=.{1,254}$)(?=.{1,64}@)[A-Za-z0-9](?:[A-Za-z0-9._%+-]{0,62}[A-Za-z0-9])?@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,63}$")
            .WithMessage("Email is invalid.");

        /*RuleFor(x => x.Payload.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");*/

        /*RuleFor(x => x.Payload.RoleIds)
            .NotEmpty()
            .WithMessage("At least one role is required.");*/
    }
}