using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

public class GetApiKeyGridResponse : BaseResponse <GetApiKeyGridResult>
{

}

public class GetApiKeyGridResult
{
    public List<GetApiKeyGridData> Items { get; set; } = [];
    public PagingInfo Paging { get; set; } = new();
}