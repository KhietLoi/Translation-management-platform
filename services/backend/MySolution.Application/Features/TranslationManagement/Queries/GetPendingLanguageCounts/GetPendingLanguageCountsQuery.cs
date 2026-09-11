    using MediatR;

    namespace MySolution.Application.Features.TranslationManagement.Queries.GetPendingLanguageCounts;

    public class GetPendingLanguageCountsQuery : IRequest<GetPendingLanguageCountsResponse>
    {
        public Guid ProjectId { get; set; }
        public Guid NamespaceId { get; set; }
    }