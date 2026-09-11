using FluentValidation;

namespace MySolution.Application.Features.Project.Commands.CreateProject;

public class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must not exceed 100 characters.");
        
        RuleFor(x => x.Payload.Description)
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");
    }
}