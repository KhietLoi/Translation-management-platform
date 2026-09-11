using FluentValidation;

namespace MySolution.Application.Features.Application.Command.UpdateApplication;

public class UpdateApplicationValidator : AbstractValidator<UpdateApplicationCommand>
{
    public UpdateApplicationValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(x => x.Payload.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }
}