using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Application.Features.User.Queries.GetUser;

namespace MySolution.Application.Features.Roles.Queries.GetRoles;

/// <summary>
/// Handler for processing the GetRolesQuery,
/// </summary>
public class GetRolesHandler : IRequestHandler<GetRolesQuery, GetRolesResponse>
{
    private readonly ILogger<GetUsersHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetRolesHandler(ILogger<GetUsersHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetRolesResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetUsersHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetRolesResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };
        
        try
        {
            var query = _unitOfWork.Role.GetAll().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(payload.Search))
            {
                query = query.Where(x =>
                    x.Name.Contains(payload.Search) ||
                    (x.Description != null && x.Description.Contains(payload.Search)));
            }

            var totalItem = await query.CountAsync(cancellationToken);

            var roles = await query
                .OrderBy(x => x.Name)
                .Skip((payload.Page - 1) * payload.Limit)
                .Take(payload.Limit)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetRolesResult
            {
                Roles = roles.Select(x => new GetRoleData
                {
                    Id = x.Id,
                    Name = x.Name,
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
