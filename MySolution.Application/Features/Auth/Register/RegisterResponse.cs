using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Auth.Register;

public class RegisterResponse : BaseResponse<RegisterResult>
{
    
}

public class RegisterResult
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}