using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Users.Commands.CreateUser;

public class CreateUserResponse : BaseResponse <CreateUserData>
{
}
public class CreateUserData
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } =  string.Empty;
    public bool IsActive { get; set; }  = true;
}

