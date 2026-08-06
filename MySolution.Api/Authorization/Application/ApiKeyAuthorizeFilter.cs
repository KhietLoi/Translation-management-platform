using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySolution.Application.Common.Interfaces.Authentication;

namespace MySolution.Api.Authorization.Application;

public class ApiKeyAuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly IApiKeyValidator _apiKeyValidator;
    public ApiKeyAuthorizeFilter(IApiKeyValidator apiKeyValidator)
    {
        _apiKeyValidator = apiKeyValidator;
    }


    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers["X-API-KEY"].FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var apiKeyContext = await _apiKeyValidator.ValidateAsync(apiKey);
        if (apiKeyContext == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        context.HttpContext.Items["ApiKey"] = apiKeyContext;
    }
}