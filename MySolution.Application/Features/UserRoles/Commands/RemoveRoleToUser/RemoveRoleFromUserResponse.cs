using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;

public class RemoveRoleFromUserResponse : BaseResponse <RemoveRoleFromUserData>
{
}

public class RemoveRoleFromUserData
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}