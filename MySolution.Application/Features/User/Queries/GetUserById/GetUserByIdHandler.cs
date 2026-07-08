using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdHandler :IRequestHandler<GetUserByIdQuery,GetUserByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserByIdHandler> _logger;
    public GetUserByIdHandler(IUnitOfWork unitOfWork,  ILogger<GetUserByIdHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetUserByIdHandler)}";
        _logger.LogInformation(functionName);

        var response = new GetUserByIdResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            var user = await _unitOfWork.User.GetUserWithRolesAsync(payload.Id);

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
                IsActive = user.IsActive,
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return response;
    }
}