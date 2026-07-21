using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MySolution.Application.Common.Interfaces.Authentication;

namespace MySolution.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    
    public PermissionAuthorizationHandler(IPermissionService permissionService , ILogger<PermissionAuthorizationHandler> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }
    
    protected  override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        /*// Check if the user is authenticated
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }*/
        Console.WriteLine("HANDLER STEP 1");
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        foreach (var claim in context.User.Claims)
        {
            _logger.LogInformation(
                "CLAIM TYPE = {Type} | VALUE = {Value}",
                claim.Type,
                claim.Value);
        }
        if (userIdClaim == null)
        {
            _logger.LogWarning("User ID claim not found in the token.");
            return;
        }
        var userId = Guid.Parse(userIdClaim!.Value);
        _logger.LogInformation("HANDLER STEP 2");
        // Check if the user has the required permission
        var permissions = await _permissionService.GetPermissionsAsync(userId);
        _logger.LogInformation($"{permissions.Count} permissions retrieved for user {userId}");
        _logger.LogInformation("HANDLER STEP 3");
        // Check if the required permission is in the user's permissions
        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}