using FluentValidation;

namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Payload.FullName)
            .NotEmpty()
            .WithMessage("{PropertyName} must not be empty")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must not exceed 100 characters");
        RuleFor(x => x.Payload.BirthDate)
            .NotEmpty()
            .WithMessage("{PropertyName} must not be empty");
        RuleFor(x => x.Payload.PhoneNumber)
            .NotEmpty()
            .WithMessage("{PropertyName} must not be empty")
            .MaximumLength(20)
            .WithMessage("{PropertyName} must not exceed 20 characters");
    }   
}