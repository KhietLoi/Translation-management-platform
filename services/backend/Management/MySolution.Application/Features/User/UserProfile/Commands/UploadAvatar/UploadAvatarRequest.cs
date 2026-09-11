using Microsoft.AspNetCore.Http;

namespace MySolution.Application.Features.User.UserProfile.Commands.UploadAvatar;

public class UploadAvatarRequest
{
    public IFormFile AvatarFile { get; set; } = null!;
}