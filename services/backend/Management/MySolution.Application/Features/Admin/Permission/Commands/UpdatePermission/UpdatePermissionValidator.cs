using FluentValidation;

namespace MySolution.Application.Features.Admin.Permission.Commands.UpdatePermission;

public class UpdatePermissionValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionValidator()
    {
        RuleFor(x => x.Payload.Code)
            .NotEmpty()
            .WithMessage("{Code} is required")
            .MaximumLength(200)
            .WithMessage("{Code} must not exceed 200 characters");
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500)
            .WithMessage("{Description} must not exceed 500 characters");
    }
}