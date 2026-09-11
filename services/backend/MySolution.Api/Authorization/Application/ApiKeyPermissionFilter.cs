using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Api.Authorization.Application;

public class ApiKeyPermissionFilter : IAsyncAuthorizationFilter
{
    private readonly ApiKeyPermissionType _permission;

    public ApiKeyPermissionFilter(ApiKeyPermissionType permission)
    {
        _permission = permission;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKeyContext = context.HttpContext.Items["ApiKey"] as ApiKeyContext;
        if (apiKeyContext == null)
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        var hasPermission = apiKeyContext.Permissions.Contains(_permission);
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }
        
        return Task.CompletedTask;
    }
}