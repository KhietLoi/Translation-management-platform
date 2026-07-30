using FluentValidation;

namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsValidator : AbstractValidator<UpdateRolePermissionsRequest>
{
    public UpdateRolePermissionsValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role id is required.");

        RuleFor(x => x.PermissionIds)
            .NotNull()
            .WithMessage("Please provide a list of permission ids.");
    }
}