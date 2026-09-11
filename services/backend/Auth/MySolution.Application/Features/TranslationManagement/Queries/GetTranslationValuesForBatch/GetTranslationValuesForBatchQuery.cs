using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValuesForBatch;

public class GetTranslationValuesForBatchQuery : IRequest<GetTranslationValuesForBatchResponse>
{
    public GetTranslationValuesForBatchRequest Payload { get; set; }

    public GetTranslationValuesForBatchQuery(GetTranslationValuesForBatchRequest payload)
    {
        Payload = payload;
    }
}