using FluentValidation;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectNamespace;

public class UpdateProjectNamespaceValidator : AbstractValidator<UpdateProjectNamespaceCommand>
{
    public UpdateProjectNamespaceValidator()
    {
        RuleFor(x => x.Payload.ProjectId)
            .NotEmpty()
            .WithMessage("Project cannot be empty");
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("Project name cannot be empty");
    }
}