using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Auth.GetCurrentUser;

public class GetCurrentUserResponse : BaseResponse <GetCurrentUserData>
{
    
}

public class GetCurrentUserData
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}