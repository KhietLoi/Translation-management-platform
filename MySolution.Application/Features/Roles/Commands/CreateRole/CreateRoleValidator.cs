using FluentValidation;
using MySolution.Application.Features.Roles.Commands.CreateRole;

namespace MySolution.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500);
    }
    
}