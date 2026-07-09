using FluentValidation;
using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Permission.Queries.GetPermissions;

public class GetPermissionsValidator : AbstractValidator<GetPermissionsQuery>
{
    public GetPermissionsValidator()
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