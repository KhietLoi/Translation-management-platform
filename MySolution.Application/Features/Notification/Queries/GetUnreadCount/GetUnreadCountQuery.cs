using MediatR;

namespace MySolution.Application.Features.Notification.Queries.GetUnreadCount;

public class GetUnreadCountQuery : IRequest<GetUnreadCountResponse>
{
    public GetUnreadCountRequest Payload { get; set; }

    public GetUnreadCountQuery(GetUnreadCountRequest payload)
    {
        Payload = payload;
    }
}