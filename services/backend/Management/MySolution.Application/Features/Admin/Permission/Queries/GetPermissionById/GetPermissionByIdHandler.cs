using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Admin.Permission.Queries.GetPermissionById;

/// <summary>
///     Handler for the GetPermissionByIdQuery, responsible for retrieving a permission by its ID.
/// </summary>
public class GetPermissionByIdHandler : IRequestHandler<GetPermissionByIdQuery, GetPermissionByIdResponse>
{
    private readonly ILogger<GetPermissionByIdHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetPermissionByIdHandler(IUnitOfWork unitOfWork, ILogger<GetPermissionByIdHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GetPermissionByIdResponse> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetPermissionByIdHandler)}";
        _logger.LogInformation(functionName);

        var response = new GetPermissionByIdResponse();

        try
        {
            var permission = await _unitOfWork.Permission
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PermissionId, cancellationToken);
            
            if (permission == null)
            {
                _logger.LogInformation("{FunctionName} Permission with ID {PermissionId} not found.", functionName, request.PermissionId);
                
                response.ErrorMessage = "Permission not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetPermissionByIdData
            {
                Id = permission.Id,
                Code = permission.Code,
                Description = permission.Description,
                CreatedDate = permission.CreatedAt,
                UpdatedDate = permission.UpdatedAt
            };
            
            _logger.LogInformation("{FunctionName} Permission with ID {PermissionId} retrieved successfully.", functionName, request.PermissionId);
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