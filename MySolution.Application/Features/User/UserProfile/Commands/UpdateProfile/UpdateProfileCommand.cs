using MediatR;

namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<UpdateProfileResponse>
{
    public UpdateProfileRequest Payload { get; set; }
    public Guid Id { get; set; }
    public UpdateProfileCommand(Guid id, UpdateProfileRequest payload)
    {
        Id = id;
        Payload = payload;
    }
}