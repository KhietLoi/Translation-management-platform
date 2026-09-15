using FluentValidation;
using FluentValidation.Validators;

namespace MySolution.Application.Features.Language.Commands.CreateLanguage;

public class CreateLanguageValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageValidator()
    {
        RuleFor(x => x.Payload.Code)
            .NotEmpty()
            .WithMessage("Language code cannot be empty")
            .MaximumLength(30)
            .WithMessage("Language code cannot exceed 30 characters");
        
        RuleFor(x => x.Payload.Name)
            .NotEmpty()
            .WithMessage("Language name cannot be empty")
            .MaximumLength(100)
            .WithMessage("Language name cannot exceed 100 characters");
    }
}