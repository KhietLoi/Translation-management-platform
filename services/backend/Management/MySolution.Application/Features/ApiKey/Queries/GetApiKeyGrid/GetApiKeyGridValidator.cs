using FluentValidation;

namespace MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

public class GetApiKeyGridValidator : AbstractValidator<GetApiKeyGridQuery>
{
    public GetApiKeyGridValidator()
    {
        RuleFor(x => x.Payload.Page)
            .GreaterThan(0);
        RuleFor(x => x.Payload.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
        RuleFor(x => x.Payload.Keyword)
            .MaximumLength(100);
    }
}