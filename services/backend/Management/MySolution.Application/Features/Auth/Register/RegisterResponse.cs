using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Auth.Register;

/// <summary>
///     Response for register operation
/// </summary>
public class RegisterResponse : BaseResponse<RegisterResult>
{
}

public class RegisterResult
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}