using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.User.Queries.GetUserById;

/// <summary>
///     Handler for retrieving a user by their ID.
/// </summary>
public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    private readonly ILogger<GetUserByIdHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdHandler(IUnitOfWork unitOfWork, ILogger<GetUserByIdHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetUserByIdHandler)}";
        _logger.LogInformation(functionName);
        var response = new GetUserByIdResponse();

        try
        {
            var user = await _unitOfWork.User.GetUserWithRolesAsync(request.Id);
            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetUserByIdData
            {
                Username = user.Username,
                Email = user.Email,
                Status =  user.Status,
                CreatedAt = user.CreatedAt,
                Roles = user.UserRoles
                    .Select(x => new RoleData
                    {
                        RoleId = x.Role.Id,
                        RoleName = x.Role.Name,
                        Description = x.Role.Description
                    })
                    .ToList(),
                Permissions = user.UserRoles
                    .SelectMany(x => x.Role.RolePermissions)
                    .Select(x => x.Permission)
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .Select(x => new PermissionData
                    {
                        PermissionId = x.Id,
                        PermissionCode = x.Code,
                        PermissionDescription = x.Description
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