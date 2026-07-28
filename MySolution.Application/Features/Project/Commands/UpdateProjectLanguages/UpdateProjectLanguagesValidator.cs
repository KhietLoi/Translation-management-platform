using FluentValidation;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectLanguages;

public class UpdateProjectLanguagesValidator : AbstractValidator<UpdateProjectLanguagesCommand>
{
    public UpdateProjectLanguagesValidator()
    {
        RuleFor(x => x.Payload.LanguagesIds)
            .NotEmpty()
            .WithMessage("Language list can't be empty.");
    }
}