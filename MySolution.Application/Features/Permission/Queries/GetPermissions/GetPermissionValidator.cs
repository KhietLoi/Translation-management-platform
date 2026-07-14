using FluentValidation;
using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Permission.Queries.GetPermissions;

/// <summary>
/// Validator for the GetPermissionsQuery class, ensuring that the query parameters are valid.
/// </summary>
public class GetPermissionsValidator : AbstractValidator<GetPermissionsQuery>
{
    public GetPermissionsValidator()
    {
        RuleFor(x => x.Payload.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1.");
        RuleFor(x => x.Payload.Limit)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Limit must be greater than or equal to 1.");
        RuleFor(x => x.Payload.Search)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Payload.Search));
    }
}