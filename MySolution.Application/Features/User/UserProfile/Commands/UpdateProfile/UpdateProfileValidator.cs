using FluentValidation;

namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        // Add validation rules here if needed
    }
}