using Microsoft.AspNetCore.SignalR;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Models.Realtime;

namespace MySolution.Infrastructure.Realtime.Hubs;

public class TranslationHub :Hub
{
    private readonly IPresenceService  _presenceService;
    private readonly ITranslationLockService  _translationLockService;

    public TranslationHub(IPresenceService presenceService, ITranslationLockService translationLockService)
    {
        _presenceService = presenceService;
        _translationLockService = translationLockService;
    }
    
    
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var projectId = await _presenceService.GetProjectIdAsync(Context.ConnectionId);
        
        await _presenceService.UserDisconnectedAsync(Context.ConnectionId);
        await _translationLockService.ReleaseAllLocksByConnectionAsync(Context.ConnectionId);

        if (projectId.HasValue)
        {
            var users = await _presenceService.GetOnlineUsersAsync(projectId.Value);
            await Clients.Group(projectId.Value.ToString()).SendAsync("OnlineUsersUpdated", users);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task JoinProject(Guid projectId, Guid userId, string username)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, projectId.ToString());
        await _presenceService.UserConnectedAsync(projectId, userId, username, Context.ConnectionId);
        var onlineUsers = await _presenceService.GetOnlineUsersAsync(projectId);
        await Clients.Group(projectId.ToString()).SendAsync("OnlineUsersUpdated",onlineUsers);
    }

    public async Task LeaveProject(Guid projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectId.ToString());
        await _presenceService.UserDisconnectedAsync(Context.ConnectionId);
        var onlineUsers = await _presenceService.GetOnlineUsersAsync(projectId);
        await Clients.Group(projectId.ToString()).SendAsync("OnlineUsersUpdated", onlineUsers);
    }
    public async Task AcquireLock(Guid translationValueId, Guid userId, string username)
    {
        var success =
            await _translationLockService.AcquireLockAsync(translationValueId, userId, username, Context.ConnectionId);

        if (!success)
        {
            var existingLock = await _translationLockService.GetLockAsync(translationValueId);
            await Clients.Caller.SendAsync("LockFailed",existingLock);
            return;
        }

        await Clients.All.SendAsync("TranslationLocked",
            new TranslationsLockInfo
            {
                TranslationValueId = translationValueId,
                UserId = userId,
                Username = username,
                ConnectionId = Context.ConnectionId,
                LockedAt = DateTime.UtcNow
            });
    }

    public async Task ReleaseLock(Guid translationValueId, Guid userId)
    {
        await _translationLockService.ReleaseLockAsync(translationValueId, userId);
        await Clients.All.SendAsync("TranslationUnlocked", translationValueId);
    }
    
    //Create group for translation value
    public async Task JoinTranslationValueGroup(Guid translationValueId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetTranslationGroupName(translationValueId));
    }
    
    public async Task LeaveTranslationValueGroup(Guid translationValueId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetTranslationGroupName(translationValueId));
    }
    
    private static string GetTranslationGroupName(Guid translationValueId)
    {
        return $"translation-value: {translationValueId}";
    }

 
    public async Task Typing(Guid translationValueId, string value)
    {
        var lockInfo = await _translationLockService.GetLockAsync(translationValueId);
        if (lockInfo == null)
        {
            return;
        }
        await Clients.OthersInGroup(GetTranslationGroupName(translationValueId))
            .SendAsync("UserTyping", translationValueId, lockInfo.UserId, lockInfo.Username, value);
    }

}   
