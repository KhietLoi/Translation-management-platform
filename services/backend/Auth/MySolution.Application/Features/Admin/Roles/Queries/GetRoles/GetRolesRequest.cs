namespace MySolution.Application.Features.Admin.Roles.Queries.GetRoles;

/// <summary>
///     Request model for retrieving roles with pagination and optional search criteria.
/// </summary>
public class GetRolesRequest
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
}