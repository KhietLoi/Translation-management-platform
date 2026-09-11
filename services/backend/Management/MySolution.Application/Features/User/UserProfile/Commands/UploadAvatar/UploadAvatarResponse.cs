using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.User.UserProfile.Commands.UploadAvatar;

public class UploadAvatarResponse : BaseResponse <UploadAvatarData>
{

}

public class UploadAvatarData
{
    public Guid UserId { get; set; }
    public string AvatarBlobName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}