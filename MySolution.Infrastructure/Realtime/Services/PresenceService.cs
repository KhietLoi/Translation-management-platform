using System.Collections.Concurrent;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Models.Realtime;
using MySolution.Infrastructure.Realtime.Models;

namespace MySolution.Infrastructure.Realtime.Services;

public class PresenceService : IPresenceService
{
    private static readonly ConcurrentDictionary<Guid, List<OnlineUser>> OnlineUsers = new();
    private static readonly ConcurrentDictionary<string, ProjectConnection> Connections = new();

    public Task UserConnectedAsync(Guid projectId, Guid userId, string username, string connectionId)
    {
        var users = OnlineUsers.GetOrAdd(projectId, _ => new List<OnlineUser>());
        lock (users)
        {
            if (users.All(x => x.UserId != userId))
            {
                users.Add(new OnlineUser
                {
                    UserId = userId,
                    Username = username,
                    ConnectedAt = DateTime.UtcNow
                });
            }
        }

        Connections.TryAdd(connectionId,
            new ProjectConnection
            {
                ProjectId = projectId,
                UserId = userId,
                ConnectionId = connectionId
            });

        return Task.CompletedTask;
    }

    public Task UserDisconnectedAsync(string connectionId)
    {
        if (!Connections.TryRemove(connectionId, out var connection))
        {
            return Task.CompletedTask;
        }

        if (!OnlineUsers.TryGetValue(connection.ProjectId, out var users))
        {
            return Task.CompletedTask;
        }
        
        var stillConnected = Connections.Values.Any (x =>
            x.ProjectId == connection.ProjectId &&
            x.UserId == connection.UserId);

        if (stillConnected)
        {
            return Task.CompletedTask;
        }

        lock (users)
        {
            users.RemoveAll(x => x.UserId == connection.UserId);
        }

        return Task.CompletedTask;
    }

    public Task<List<OnlineUser>> GetOnlineUsersAsync(Guid projectId)
    {
        if (OnlineUsers.TryGetValue(projectId, out var users))
        {
            return Task.FromResult(users);
        }

        return Task.FromResult(new List<OnlineUser>());
    }

    public Task<Guid?> GetProjectIdAsync(string connectionId)
    {
        if (Connections.TryGetValue(connectionId, out var connection))
        {
            return Task.FromResult<Guid?>(connection.ProjectId);
        }
        
        return Task.FromResult<Guid?>(null);
    }
}