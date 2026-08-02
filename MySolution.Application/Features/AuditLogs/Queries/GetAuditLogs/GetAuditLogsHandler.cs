using System.Net;
using MediatR;
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
            var result = await _unitOfWork.AuditLog.GetByEntityAsync(request.EntityName, request.EntityId, request.PageNumber, request.PageSize);
            response.Data = new GetAuditLogsData
            {
                TotalCount = result.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,

                Items = result.Items
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