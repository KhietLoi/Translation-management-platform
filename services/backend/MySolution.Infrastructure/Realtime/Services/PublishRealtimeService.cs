using Microsoft.AspNetCore.SignalR;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Models.Realtime;
using MySolution.Infrastructure.Realtime.Hubs;

namespace MySolution.Infrastructure.Realtime.Services;

public class PublishRealtimeService : IPublishRealtimeService
{
    private readonly IHubContext<TranslationHub> _hubContext;
    
    public  PublishRealtimeService(IHubContext<TranslationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task SendProgressAsync(Guid projectId, PublishProgressInfo progressInfo)
    {
        await _hubContext.Clients.Group(projectId.ToString()).SendAsync("PublishProgress", progressInfo);
    }
}