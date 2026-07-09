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
            .MaximumLength(200);
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500);
    }
}