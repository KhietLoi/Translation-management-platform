using MediatR;

namespace MySolution.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQuery : IRequest<GetAuditLogsResponse>
{
   public string EntityName { get; set; } = string.Empty;
   public Guid EntityId { get; set; }
   public int PageNumber { get; set; } = 1;
   public int PageSize { get; set; } = 20;
}