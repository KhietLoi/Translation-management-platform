using FluentValidation;

namespace MySolution.Application.Features.Notification.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadValidator()
    {
        RuleFor(x => x.Payload.NotificationId)
            .NotEmpty()
            .WithMessage("{PropertyName} can not be empty");
    }
}