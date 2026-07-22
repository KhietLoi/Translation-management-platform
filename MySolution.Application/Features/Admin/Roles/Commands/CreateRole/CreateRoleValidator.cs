using FluentValidation;
using MySolution.Application.Features.Roles.Commands.CreateRole;

namespace MySolution.Application.Features.Roles.Commands.CreateRole;

/// <summary>
/// Validator for the CreateRoleCommand,
/// ensuring that the role name is not empty and does not exceed 200 characters,
/// and that the description does not exceed 500 characters.
/// </summary>
public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("Please provide a name.")
            .MaximumLength(200)
            .WithMessage("{Name} must not exceed 200 characters");
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500)
            .WithMessage("{Description} must not exceed 500 characters");
    }
}