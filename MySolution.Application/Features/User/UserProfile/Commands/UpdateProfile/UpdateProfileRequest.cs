using MediatR;

namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileRequest 
{
    public string FullName { get; set; } = string.Empty;
    public DateOnly  BirthDate { get; set; } 
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; } 
}