using FluentValidation;

namespace MySolution.Application.Features.RolePermissions.Commands.AssignPermissionToRole;

/// <summary>
/// Validator for the AssignPermissionToRoleCommand
/// </summary>
public class AssignPermissionToRoleValidator : AbstractValidator<AssignPermissionToRoleCommand>
{
    public AssignPermissionToRoleValidator()
    {
        RuleFor(x => x.Payload.PermissionId)
            .NotEmpty();
        RuleFor(x => x.Payload.RoleId)
            .NotEmpty();
    }
}