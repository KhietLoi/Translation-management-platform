using MediatR;

namespace MySolution.Application.Features.Notification.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommand : IRequest<MarkNotificationAsReadResponse>
{
    public MarkNotificationAsReadRequest Payload { get; set; }

    public MarkNotificationAsReadCommand(MarkNotificationAsReadRequest payload)
    {
        Payload = payload;
    }
}