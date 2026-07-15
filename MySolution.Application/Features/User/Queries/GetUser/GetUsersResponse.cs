using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.User.Queries.GetUser;

/// <summary>
/// Response class for the get users operation.
/// </summary>
public class GetUsersResponse : BaseResponse
{
    public GetUsersResult? Data { get; set; }
}

public class GetUsersResult
{
    public List<GetUsersData> Users { get; set; } = [];
    public PagingInfo Paging { get; set; } = new();
}

public class GetUsersData
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}