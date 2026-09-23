using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Admin.Roles.Commands.DeleteRole;

public class DeleteRoleResponse : BaseResponse<DeleteRoleData>
{
}

public class DeleteRoleData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}