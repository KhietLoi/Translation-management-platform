using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Infrastructure.Authorization;

public class PermissionService : IPermissionService
{
    private readonly IPermissionCacheService _cached;
    private readonly ILogger<PermissionService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PermissionService(IUnitOfWork unitOfWork, IPermissionCacheService permissionCacheService,
        ILogger<PermissionService> logger)
    {
        _unitOfWork = unitOfWork;
        _cached = permissionCacheService;
        _logger = logger;
    }

    public async Task<HashSet<string>> GetPermissionsAsync(Guid userId)
    {
        var cached = await _cached.GetAsync(userId);
        if (cached is not null)
        {
            _logger.LogInformation("Permissions retrieved from cache for user: {UserId}", userId);
            return cached;
        }

        var permissions = await _unitOfWork.User
            .GetAll()
            .Where(x => x.Id == userId)
            .SelectMany(x => x.UserRoles)
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Code)
            .Distinct()
            .ToHashSetAsync();

        await _cached.SetAsync(userId, permissions, TimeSpan.FromHours(1)); // Upgrade ttl -> auto load when permission is update
        _logger.LogInformation("Permissions retrieved from database for user: {UserId}", userId);

        return permissions;
    }
}