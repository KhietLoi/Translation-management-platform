namespace MySolution.Application.Features.User.Queries.GetUser;

public class GetUsersRequest
{
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string? Search { get; set; }
    
}