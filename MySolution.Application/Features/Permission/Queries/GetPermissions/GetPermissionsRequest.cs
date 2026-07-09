namespace MySolution.Application.Features.Permission.Queries.GetPermissions;

public class GetPermissionsRequest
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
}