using FluentValidation;

namespace MySolution.Application.Features.Language.Commands.UpdateLanguage;

public class UpdateLanguageValidator : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageValidator()
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