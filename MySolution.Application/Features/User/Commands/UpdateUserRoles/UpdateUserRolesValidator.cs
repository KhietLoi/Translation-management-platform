using FluentValidation;

namespace MySolution.Application.Features.User.Commands.UpdateUserRoles;


public class UpdateUserRolesValidator 
    : AbstractValidator<UpdateUserRolesRequest>
{
    public UpdateUserRolesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User id is required.");


        RuleFor(x => x.RoleIds)
            .NotNull()
            .WithMessage("Role list is required.");
    }
}