using FluentValidation;

namespace MySolution.Application.Features.Project.Commands.CreateProjectNamespace;

public class CreateProjectNamespaceValidator : AbstractValidator<CreateProjectNamespaceCommand>
{
    public CreateProjectNamespaceValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("Project name cannot be empty");
    }
}