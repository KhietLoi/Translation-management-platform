    using MediatR;

    namespace MySolution.Application.Features.TranslationManagement.Queries.GetPendingNamespaceCounts;

    public class GetPendingNamespaceCountsQuery : IRequest<GetPendingNamespaceCountsResponse>
    {
        public Guid ProjectId { get; set; }
    }