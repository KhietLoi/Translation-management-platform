using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Admin.User.Commands.UpdateUser;

/// <summary>
///     Response class for the update user operation.
/// </summary>
public class UpdateUserResponse : BaseResponse<UpdateUserData>
{
}

public class UpdateUserData
{
    public Guid? Id { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
}