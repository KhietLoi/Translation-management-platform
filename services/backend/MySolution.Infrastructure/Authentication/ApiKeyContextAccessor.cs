using Microsoft.AspNetCore.Http;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Models;

namespace MySolution.Infrastructure.Authentication;

public class ApiKeyContextAccessor : IApiKeyContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiKeyContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ApiKeyContext? Current
    {
        get { return _httpContextAccessor.HttpContext?.Items["ApiKey"] as ApiKeyContext; }
    }
}