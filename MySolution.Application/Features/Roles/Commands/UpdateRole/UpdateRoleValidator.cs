using FluentValidation;

namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
/// Validator for the UpdateRoleCommand
/// </summary>
public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500);
    }
}