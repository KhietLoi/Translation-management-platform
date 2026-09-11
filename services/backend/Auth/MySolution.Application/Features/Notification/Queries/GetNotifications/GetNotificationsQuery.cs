using MediatR;

namespace MySolution.Application.Features.Notification.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<GetNotificationsResponse>
{
    public GetNotificationsRequest Payload { get; set; }

    public GetNotificationsQuery(GetNotificationsRequest payload)
    {
        Payload = payload;
    }
}