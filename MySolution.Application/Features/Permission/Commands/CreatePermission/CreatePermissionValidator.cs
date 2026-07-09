using FluentValidation;

namespace MySolution.Application.Features.Permission.Commands.CreatePermission;

/// <summary>
/// Validator for the CreatePermissionCommand, ensuring that the required fields are not empty and meet length constraints.
/// </summary>
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