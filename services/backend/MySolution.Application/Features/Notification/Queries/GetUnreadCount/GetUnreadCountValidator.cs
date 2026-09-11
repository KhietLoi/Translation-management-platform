using FluentValidation;

namespace MySolution.Application.Features.Notification.Queries.GetUnreadCount;

public class GetUnreadCountValidator : AbstractValidator<GetUnreadCountQuery>
{
    public GetUnreadCountValidator()
    {
        // Add validation rules here if needed
    }
}