namespace MySolution.Application.Features.User.Queries.GetUser;

/// <summary>
///     Request model for retrieving a list of users with pagination and optional search criteria
/// </summary>
public class GetUsersRequest
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
}