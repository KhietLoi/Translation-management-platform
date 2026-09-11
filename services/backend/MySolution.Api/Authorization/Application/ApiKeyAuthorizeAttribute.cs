using Microsoft.AspNetCore.Mvc;

namespace MySolution.Api.Authorization.Application;

public class ApiKeyAuthorizeAttribute : TypeFilterAttribute
{
    public ApiKeyAuthorizeAttribute()
        : base(typeof(ApiKeyAuthorizeFilter))
    {
    }
}