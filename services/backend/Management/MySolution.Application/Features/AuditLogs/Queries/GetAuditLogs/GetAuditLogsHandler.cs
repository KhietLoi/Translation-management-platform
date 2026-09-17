using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQuery, GetAuditLogsResponse>
{
    private readonly ILogger<GetAuditLogsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetAuditLogsHandler
    (
        ILogger<GetAuditLogsHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetAuditLogsQuery, GetAuditLogsResponse>

    public async Task<GetAuditLogsResponse> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetAuditLogsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetAuditLogsResponse();

        try
        {
            var query = _unitOfWork.AuditLog
                .GetAll()
                .AsNoTracking()
                .Where(x =>
                    x.EntityName == request.EntityName &&
                    x.EntityId == request.EntityId);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetAuditLogsData
            {
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,

                Items = items
                    .Select(x => new AuditLogItem
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        UserName = x.User.Username,
                        Action = x.Action,
                        EntityName = x.EntityName,
                        EntityId = x.EntityId,
                        OldValue = x.OldValue,
                        NewValue = x.NewValue,
                        CreatedAt = x.CreatedAt
                    })
                    .ToList()
            };
            
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}