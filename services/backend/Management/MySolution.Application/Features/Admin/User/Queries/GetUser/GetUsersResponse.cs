using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Admin.User.Queries.GetUser;


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
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}