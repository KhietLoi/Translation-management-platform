using FluentValidation;

namespace MySolution.Application.Features.User.Queries.GetUser;

/// <summary>
///     Validator for the GetUsersQuery
/// </summary>
public class GetUsersValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersValidator()
    {
        RuleFor(x => x.Payload.Page)
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.Payload.Limit)
            .GreaterThanOrEqualTo(1);
        RuleFor(x => x.Payload.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Payload.Search));
    }
}