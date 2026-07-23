using MediatR;

namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<UpdateProfileResponse>
{
    public UpdateProfileRequest Payload { get; set; }

    public UpdateProfileCommand(UpdateProfileRequest payload)
    {
        Payload = payload;
    }
}