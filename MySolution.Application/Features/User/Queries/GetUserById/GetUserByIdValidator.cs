using FluentValidation;

namespace MySolution.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Payload.Id)
            .NotEmpty();
    }
    
}