using FluentValidation;

namespace MySolution.Application.Features.Notification.Queries.GetNotificationDetail;

public class GetNotificationDetailValidator : AbstractValidator<GetNotificationDetailQuery>
{
    public GetNotificationDetailValidator()
    {
        RuleFor(x => x.Payload.NotificationId)
            .NotEmpty()
            .WithMessage("NotificationId is required.");
    }
}