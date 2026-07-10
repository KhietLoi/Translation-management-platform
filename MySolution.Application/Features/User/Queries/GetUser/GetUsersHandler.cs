using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.User.Queries.GetUser;

/// <summary>
/// Handler for processing the GetUsersQuery
/// </summary>
public class GetUsersHandler : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
    private readonly ILogger<GetUsersHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersHandler
    (
        ILogger<GetUsersHandler> logger,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
 
    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetUsersHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetUsersResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };
        
        try
        {
            var query = _unitOfWork.User
                .GetAll()
                .AsNoTracking();
            
            if (!string.IsNullOrWhiteSpace(payload.Search))
            {
                query = query.Where(x =>
                    x.Username.Contains(payload.Search) ||
                    x.Email.Contains(payload.Search));
            }
            
            var totalItem = await query.CountAsync(cancellationToken);
            var users = await query
                .OrderBy(x => x.Username)
                .Skip((payload.Page - 1) * payload.Limit)
                .Take(payload.Limit)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetUsersResult
            {
                Users = users.Select(x => new GetUsersData
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
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