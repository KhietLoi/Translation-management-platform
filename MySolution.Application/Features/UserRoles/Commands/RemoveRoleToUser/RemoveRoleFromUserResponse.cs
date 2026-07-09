using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;
/// <summary>
/// Response class for the remove role from user operation.
/// </summary>
public class RemoveRoleFromUserResponse : BaseResponse <RemoveRoleFromUserData>
{
}

public class RemoveRoleFromUserData
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}