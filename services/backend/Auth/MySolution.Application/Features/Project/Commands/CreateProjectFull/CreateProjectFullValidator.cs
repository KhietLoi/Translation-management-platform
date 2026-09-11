using FluentValidation;

namespace MySolution.Application.Features.Project.Commands.CreateProjectFull;

public class CreateProjectFullValidator : AbstractValidator<CreateProjectFullCommand>
{
    public CreateProjectFullValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("Project name is required.");
        RuleFor(x => x.Payload.LanguageIds)
            .NotNull()
            .WithMessage("At least one programming language is required.");
        RuleFor(x => x.Payload.MemberIds)
            .NotNull()
            .WithMessage("At least one team member is required.");
    }
}