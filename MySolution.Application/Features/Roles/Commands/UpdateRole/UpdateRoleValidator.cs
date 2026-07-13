using FluentValidation;

namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
/// Validator for the UpdateRoleCommand
/// </summary>
public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("Please provide a name.")
            .MaximumLength(200)
            .WithMessage("{Name} must not exceed 200 characters");
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500)
            .WithMessage("{Description} must not exceed 500 characters");
    }
}