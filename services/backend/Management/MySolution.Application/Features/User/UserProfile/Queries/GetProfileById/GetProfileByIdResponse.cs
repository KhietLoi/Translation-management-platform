using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.User.UserProfile.Queries.GetProfileById;

public class GetProfileByIdResponse : BaseResponse <GetProfileByIdData>
{
}

public class GetProfileByIdData
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; } 
    public DateOnly? BirthDate { get; set; }
    public string? PhoneNumber { get; set; } 
    public string? AvatarBlobName { get; set; }
    public string? Address { get; set; }
}