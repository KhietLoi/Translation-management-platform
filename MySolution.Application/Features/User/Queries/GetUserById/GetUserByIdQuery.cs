using MediatR;

namespace MySolution.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
{
    public GetUserByIdRequest Payload { get; }

    public GetUserByIdQuery(GetUserByIdRequest payload)
    {
        Payload = payload;
    }
}