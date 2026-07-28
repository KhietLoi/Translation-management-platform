using FluentValidation;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectMembers;

public class UpdateProjectMembersValidator : AbstractValidator<UpdateProjectMembersCommand>
{
    public UpdateProjectMembersValidator()
    {
        RuleFor(x => x.Payload.Members)
            .NotEmpty()
            .WithMessage("Member list can't be empty.");
    }
}