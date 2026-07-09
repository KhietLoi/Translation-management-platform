using MySolution.Application.Common.Model;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Roles.Queries.GetRoles;

/// <summary>
/// Response class for the get roles operation.
/// </summary>
public class GetRolesResponse : BaseResponse <GetRolesResult>
{
}

public class GetRolesResult
{
    public List<GetRoleData> Roles { get; set; } = [];
    public PagingInfo Paging { get; set; } = new();
}

public class GetRoleData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}