using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Admin.Permission.Commands.UpdatePermission;

public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, UpdatePermissionResponse>
{
    private readonly ILogger<UpdatePermissionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePermissionHandler
    (
        IUnitOfWork unitOfWork,
        ILogger<UpdatePermissionHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdatePermissionResponse> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdatePermissionHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdatePermissionResponse();

        try
        {
            // Check if permission exists
            var permission = await _unitOfWork.Permission
                .GetAll()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (permission == null)
            {
                _logger.LogInformation("{FunctionName} Permission not found.", functionName);
                response.ErrorMessage = "Permission not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            // Check if code exists:
            var existingPermission = await _unitOfWork.Permission
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == payload.Code, cancellationToken);
            
            if (existingPermission != null && existingPermission.Id != permission.Id)
            {
                _logger.LogInformation("{FunctionName} Permission code already exists.", functionName);
                response.ErrorMessage = "Permission code already exists.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            //Update
            permission.Code = payload.Code;
            permission.Description = payload.Description;
            permission.UpdatedAt = DateTime.UtcNow;
            
            //Save
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new UpdatePermissionData
            {
                Id = permission.Id,
                Code = permission.Code,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            };
            
            _logger.LogInformation("{FunctionName} Permission updated successfully.", functionName);
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