using FluentValidation;

namespace MySolution.Application.Features.Notification.Commands.MarkAllNotificationAsRead;

public class MarkAllNotificationAsReadValidator : AbstractValidator<MarkAllNotificationAsReadCommand>
{
    public MarkAllNotificationAsReadValidator()
    {
        // Add validation rules here if needed
    }
}