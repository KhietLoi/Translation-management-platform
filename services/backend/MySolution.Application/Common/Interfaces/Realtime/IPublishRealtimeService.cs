using MySolution.Application.Common.Models.Realtime;

namespace MySolution.Application.Common.Interfaces.Realtime;

public interface IPublishRealtimeService
{
    Task SendProgressAsync(Guid projectId, PublishProgressInfo progressInfo);
}