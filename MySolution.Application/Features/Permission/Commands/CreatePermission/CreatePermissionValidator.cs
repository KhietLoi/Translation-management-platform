using FluentValidation;

namespace MySolution.Application.Features.Permission.Commands.CreatePermission;

public class CreatePermissionValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionValidator()
    {
        RuleFor(x => x.Payload.Code)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500);
    }
}