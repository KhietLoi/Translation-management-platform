using FluentValidation;

namespace MySolution.Application.Features.Permission.Commands.UpdatePermission;

public class UpdatePermissionValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionValidator()
    {
        RuleFor(x => x.Payload.Code)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500);
    }
}