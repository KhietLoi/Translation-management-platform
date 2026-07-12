using FluentValidation;

namespace MySolution.Application.Features.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsValidator : AbstractValidator<UpdateRolePermissionsRequest>
{
    public UpdateRolePermissionsValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role id is required.");

        RuleFor(x => x.PermissionIds)
            .NotNull()
            .WithMessage("Permission list is required.");
    }
}