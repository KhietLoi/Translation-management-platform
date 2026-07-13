using Microsoft.AspNetCore.Authorization;

namespace MySolution.Api.Authorization;

public class PermissionRequirement: IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}