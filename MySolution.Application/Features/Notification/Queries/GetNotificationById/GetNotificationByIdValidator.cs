using FluentValidation;

namespace MySolution.Application.Features.Notification.Queries.GetNotificationById;

public class GetNotificationByIdValidator : AbstractValidator<GetNotificationByIdQuery>
{
    public GetNotificationByIdValidator()
    {
        // Add validation rules here if needed
    }
}