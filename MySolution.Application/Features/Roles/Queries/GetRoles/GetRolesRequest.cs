namespace MySolution.Application.Features.Roles.Queries.GetRoles;

public class GetRolesRequest
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
}