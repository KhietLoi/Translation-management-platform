using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Admin.Permission.Commands.CreatePermission;

/// <summary>
///     Handler for creating a new permission.
/// </summary>
public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, CreatePermissionResponse>
{
    private readonly ILogger<CreatePermissionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePermissionHandler
    (
        IUnitOfWork unitOfWork,
        ILogger<CreatePermissionHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreatePermissionResponse> Handle(CreatePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreatePermissionHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreatePermissionResponse();

        try
        {
            // Validate if the permission code already exists
            if (await _unitOfWork.Permission.ExistsByCodeAsync(payload.Code))
            {
                response.ErrorMessage = "Permission code already exists";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            //Create Permission
            var permission = new Domain.Entities.Permission
            {
                Id = Guid.CreateVersion7(),
                Code = payload.Code,
                Description = payload.Description,
                CreatedAt = DateTime.UtcNow
            };

            //Save
            await _unitOfWork.Permission.Add(permission);
            await _unitOfWork.SaveAsync(cancellationToken);
            //Response
            response.Data = new CreatePermissionData
            {
                Code = permission.Code,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.Created);
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