using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Roles.Queries.GetRoleById;

/// <summary>
/// Handler for the GetRoleByIdQuery, responsible for retrieving a role by its ID.
/// </summary>
public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, GetRoleByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetRoleByIdHandler> _logger;

    public GetRoleByIdHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetRoleByIdHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GetRoleByIdResponse> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetRoleByIdHandler)}";
        _logger.LogInformation(functionName);

        var response = new GetRoleByIdResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            var role = await _unitOfWork.Role.GetByIdAsync(request.RoleId);

            if (role == null)
            {
                response.ErrorMessage = "Role not found.";
                response.WithStatus(HttpStatusCode.NotFound);

                return response;
            }

            var permissions = await _unitOfWork.Role
                .GetPermissionsAsync(role.Id);
            response.Data = new GetRoleByIdData
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = permissions
                    .Select(x => new GetRolePermissionData
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Description = x.Description
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
}