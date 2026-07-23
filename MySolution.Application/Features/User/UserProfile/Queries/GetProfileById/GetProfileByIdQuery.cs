using MediatR;

namespace MySolution.Application.Features.User.UserProfile.Queries.GetProfileById;

public class GetProfileByIdQuery : IRequest<GetProfileByIdResponse>
{
    public Guid Id { get; set; }

    public GetProfileByIdQuery (Guid id)
    {
        Id = id;
    }
}