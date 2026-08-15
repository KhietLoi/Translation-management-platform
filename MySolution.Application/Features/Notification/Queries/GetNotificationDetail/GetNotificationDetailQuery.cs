using MediatR;

namespace MySolution.Application.Features.Notification.Queries.GetNotificationDetail;

public class GetNotificationDetailQuery : IRequest<GetNotificationDetailResponse>
{
    public GetNotificationDetailRequest Payload { get; set; }

    public GetNotificationDetailQuery(GetNotificationDetailRequest payload)
    {
        Payload = payload;
    }
}