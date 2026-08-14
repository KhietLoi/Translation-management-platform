using MediatR;

namespace MySolution.Application.Features.Notification.Queries.GetNotificationById;

public class GetNotificationByIdQuery : IRequest<GetNotificationByIdResponse>
{
    public GetNotificationByIdRequest Payload { get; set; }

    public GetNotificationByIdQuery(GetNotificationByIdRequest payload)
    {
        Payload = payload;
    }
}