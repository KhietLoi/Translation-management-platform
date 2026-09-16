using System.Data;
using FluentValidation;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchUpdateTranslation;

public class BatchUpdateTranslationValidator : AbstractValidator<BatchUpdateTranslationCommand>
{
    public BatchUpdateTranslationValidator()
    {
        RuleFor (x => x.Payload)
            .NotEmpty()
            .WithMessage("Payload items must not be empty");
        RuleFor (x => x.Payload.Items)
            .NotEmpty()
            .WithMessage("Payload items must not be empty");
        RuleFor (x => x.Payload.LanguageId)
            .NotEmpty()
            .WithMessage("LanguageId must not be empty");
        RuleFor(x => x.Payload.NamespaceId)
            .NotEmpty()
            .WithMessage("NamespaceId must not be empty");
        RuleFor(x => x.Payload.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId must not be empty");
    }
}