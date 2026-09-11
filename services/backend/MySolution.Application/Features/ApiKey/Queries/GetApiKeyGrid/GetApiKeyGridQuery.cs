using MediatR;

namespace MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

public class GetApiKeyGridQuery : IRequest<GetApiKeyGridResponse>
{
    public GetApiKeyGridRequest Payload { get; set; }

    public GetApiKeyGridQuery(GetApiKeyGridRequest payload)
    {
        Payload = payload;
    }
}