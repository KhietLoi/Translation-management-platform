using Microsoft.AspNetCore.Mvc;
using MySolution.Domain.Enums;

namespace MySolution.Api.Authorization.Application;

public class ApiKeyPermissionAttribute : TypeFilterAttribute
{
    public ApiKeyPermissionAttribute( ApiKeyPermissionType permission) : base(typeof(ApiKeyPermissionFilter))
    {
        Arguments = [permission];
    }   
}