using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.User.Commands.UpdateUser;

/// <summary>
/// Response class for the update user operation.
/// </summary>
public class UpdateUserResponse : BaseResponse <UpdateUserData>
{ 
}
public class UpdateUserData
{
    public Guid? Id { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } =  string.Empty;
    public bool? IsActive { get; set; }  = true;
}