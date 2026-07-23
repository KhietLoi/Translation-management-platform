using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Auth.VerifyEmail;

public class VerifyEmailResponse : BaseResponse<VerifyEmailData>
{
}

public class VerifyEmailData
{
    public string Email { get; set; } = string.Empty;
}