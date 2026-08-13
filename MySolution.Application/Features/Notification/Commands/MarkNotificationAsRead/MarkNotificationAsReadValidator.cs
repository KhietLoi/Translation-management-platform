using FluentValidation;

namespace MySolution.Application.Features.Notification.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadValidator()
    {
        // Add validation rules here if needed
    }
}