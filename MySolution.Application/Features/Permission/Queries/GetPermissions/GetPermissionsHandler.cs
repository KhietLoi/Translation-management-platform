using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Model;
using MySolution.Application.Features.User.Queries.GetUser;

namespace MySolution.Application.Features.Permission.Queries.GetPermissions;

/// <summary>
/// Handler for processing the GetPermissionsQuery,
/// which retrieves a list of permissions based on the provided search criteria and pagination parameters.
/// </summary>
public class GetPermissionsHandler : IRequestHandler <GetPermissionsQuery,GetPermissionsResponse>
{
    private readonly ILogger<GetUsersHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetPermissionsHandler(ILogger<GetUsersHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPermissionsResponse> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetUsersHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetPermissionsResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            var query = _unitOfWork.Permission
                .GetAll()
                .AsNoTracking();
            
            if (!string.IsNullOrWhiteSpace(payload.Search))
            {
                query = query.Where(x =>
                    x.Code.Contains(payload.Search) ||
                    (x.Description != null &&
                     x.Description.Contains(payload.Search)));
            }
            
            var totalItem = await query.CountAsync(cancellationToken);
            
            var permissions = await query
                .OrderBy(x => x.Code)
                .Skip((payload.Page - 1) * payload.Limit)
                .Take(payload.Limit)
                .ToListAsync(cancellationToken);
            response.Data = new GetPermissionsResult
            {
                Permissions = permissions.Select(x => new GetPermissionsData
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).ToList(),
                Paging = new PagingInfo
                {
                    Page = payload.Page,
                    Limit = payload.Limit,
                    TotalItem = totalItem,
                    TotalPage = (int)Math.Ceiling(totalItem / (double)payload.Limit)
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
}