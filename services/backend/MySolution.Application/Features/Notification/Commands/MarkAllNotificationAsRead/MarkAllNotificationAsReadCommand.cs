using MediatR;

namespace MySolution.Application.Features.Notification.Commands.MarkAllNotificationAsRead;

public class MarkAllNotificationAsReadCommand : IRequest<MarkAllNotificationAsReadResponse>
{
    public MarkAllNotificationAsReadRequest Payload { get; set; }

    public MarkAllNotificationAsReadCommand(MarkAllNotificationAsReadRequest payload)
    {
        Payload = payload;
    }
}