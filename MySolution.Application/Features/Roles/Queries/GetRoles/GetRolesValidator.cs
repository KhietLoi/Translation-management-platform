using FluentValidation;

namespace MySolution.Application.Features.Roles.Queries.GetRoles;

public class GetRolesValidator : AbstractValidator<GetRolesQuery>
{
    public GetRolesValidator()
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