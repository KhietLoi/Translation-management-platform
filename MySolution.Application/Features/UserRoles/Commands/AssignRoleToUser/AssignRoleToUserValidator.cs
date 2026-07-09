using FluentValidation;

namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Validator for the AssignRoleToUserCommand, ensuring that the required fields are provided and valid.
/// </summary>
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