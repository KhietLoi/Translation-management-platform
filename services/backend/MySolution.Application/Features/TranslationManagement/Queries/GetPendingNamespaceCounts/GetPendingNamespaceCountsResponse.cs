using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetPendingNamespaceCounts;

public class GetPendingNamespaceCountsResponse : BaseResponse<GetPendingNamespaceCountsResult>
{

}

public class GetPendingNamespaceCountsResult
{
    public List<PendingNamespaceCount> Namespaces { get; set; } = new();
}