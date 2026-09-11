using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MySolution.Application.Common.Interfaces.Authentication;

namespace MySolution.Api.Authorization.User;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IPermissionService permissionService,
        ILogger<PermissionAuthorizationHandler> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        foreach (var claim in context.User.Claims)
            _logger.LogInformation("CLAIM TYPE = {Type} | VALUE = {Value}", claim.Type, claim.Value);
        if (userIdClaim == null)
        {
            _logger.LogWarning("User ID claim not found in the token.");
            return;
        }

        var userId = Guid.Parse(userIdClaim.Value);
        // Check if the user has the required permission
        var permissions = await _permissionService.GetPermissionsAsync(userId);
        _logger.LogInformation("Permissions: {Permissions}", string.Join(",", permissions));
        
        //foreach (var permission in permissions) _logger.LogInformation("PERMISSION = {Permission}", permission);
        
        // Check if the required permission is in the user's permissions
        if (Enumerable.Contains(permissions, requirement.Permission, StringComparer.OrdinalIgnoreCase))
            context.Succeed(requirement);
    }
}