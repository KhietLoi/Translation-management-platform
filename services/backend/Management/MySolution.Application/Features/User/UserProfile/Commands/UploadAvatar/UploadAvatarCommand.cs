using MediatR;

namespace MySolution.Application.Features.User.UserProfile.Commands.UploadAvatar;

public class UploadAvatarCommand : IRequest<UploadAvatarResponse>
{
    public UploadAvatarRequest Payload { get; set; }
    public Guid UserId { get; set; }
    public UploadAvatarCommand(UploadAvatarRequest payload,  Guid userId)
    {
        Payload = payload;
        UserId = userId;
    }
}