using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileResponse : BaseResponse <UpdateData>
{
}
public class UpdateData
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly  BirthDate { get; set; } 
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AvatarBlobName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime? UpdatedAt { get; set; }
}