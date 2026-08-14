using MySolution.Application.Common.Models.Realtime;

namespace MySolution.Application.Common.Interfaces.Realtime;

public interface IPresenceService
{
    Task UserConnectedAsync(Guid projectId, Guid userId, string username, string connectionId);
    Task UserDisconnectedAsync(string connectionId);
    Task<List<OnlineUser>> GetOnlineUsersAsync(Guid projectId);
    Task<Guid?> GetProjectIdAsync (string connectionId);
}