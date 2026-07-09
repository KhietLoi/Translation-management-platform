using FluentValidation;

namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

public class AssignRoleToUserValidator : AbstractValidator <AssignRoleToUserCommand>
{
    public AssignRoleToUserValidator()
    {
        RuleFor(x => x.Payload.RoleId)
            .NotEmpty();
        RuleFor(x => x.Payload.UserId)
            .NotEmpty();
    }
}