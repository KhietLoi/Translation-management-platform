using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Notification.Queries.GetUnreadCount;

public class GetUnreadCountResponse : BaseResponse <GetUnreadCountData>
{
  
}

public class GetUnreadCountData
{
    public int Count { get; set; }
}