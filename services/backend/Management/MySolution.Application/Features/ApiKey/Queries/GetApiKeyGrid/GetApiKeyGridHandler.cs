using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
namespace MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

public class GetApiKeyGridHandler : IRequestHandler<GetApiKeyGridQuery, GetApiKeyGridResponse>
{
    private readonly ILogger<GetApiKeyGridHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetApiKeyGridHandler
    (
        ILogger<GetApiKeyGridHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetApiKeyGridQuery, GetApiKeyGridResponse>

    public async Task<GetApiKeyGridResponse> Handle(GetApiKeyGridQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetApiKeyGridHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApiKeyGridResponse();

        try
        {
            var query = _unitOfWork.ApiKey
                .GetAll()
                .AsNoTracking();
            if(payload.ProjectId.HasValue)
            {
                query = query.Where(x => x.Application.ProjectId == payload.ProjectId.Value);
            }

            if (payload.ApplicationId.HasValue)
            {
                query = query.Where(x => x.ApplicationId == payload.ApplicationId.Value);
            }
            
            if (!string.IsNullOrWhiteSpace(payload.Keyword))
            {
                query = query.Where(x =>
                    x.Name.Contains(payload.Keyword) ||
                    x.KeyPrefix.Contains(payload.Keyword));
            }

            if (payload.IsRevoked.HasValue)
            {
                query = payload.IsRevoked.Value
                    ? query.Where(x => x.RevokedAt != null)
                    : query.Where(x => x.RevokedAt == null);
            }
            
            var totalItems = await query.CountAsync(cancellationToken);
            
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((payload.Page - 1) * payload.Limit)
                .Take(payload.Limit)
                .Select(x => new GetApiKeyGridData
                {
                    Id = x.Id,
                    ProjectId = x.Application.ProjectId,
                    ProjectName = x.Application.Project.Name,
                    ApplicationId = x.ApplicationId,
                    ApplicationName = x.Application.Name,
                    Name = x.Name,
                    KeyPrefix = x.KeyPrefix,

                    Permissions = x.Permissions.Select(p => p.Permission.ToString()).ToList(),

                    IsRevoked = x.RevokedAt != null,
                    IsExpired = x.ExpiresAt.HasValue && x.ExpiresAt.Value < DateTime.UtcNow,

                    ExpiresAt = x.ExpiresAt,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            response.Data = new GetApiKeyGridResult
                {
                    Items = items,
                    Paging = new PagingInfo
                    {
                        Page = payload.Page,
                        Limit = payload.Limit,
                        TotalItem = totalItems,
                        TotalPage = (int)Math.Ceiling(totalItems / (double)payload.Limit)
                    }
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