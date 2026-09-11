using FluentValidation;

namespace MySolution.Application.Features.Notification.Queries.GetNotifications;

public class GetNotificationsValidator : AbstractValidator<GetNotificationsQuery>
{
    public GetNotificationsValidator()
    {
        RuleFor(x => x.Payload.PageNumber)
            .GreaterThan(0);
        RuleFor(x => x.Payload.PageSize)
            .InclusiveBetween(1, 100);
    }
}