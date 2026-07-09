using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Permission.Commands.UpdatePermission;

public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, UpdatePermissionResponse>
{
    private IUnitOfWork _unitOfWork;
    private ILogger<UpdatePermissionHandler> _logger;

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
        var response = new UpdatePermissionResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            //Check permission existing
            var permission = await _unitOfWork.Permission.GetPermissionByIdAsync(request.Id);
            
            if (permission == null)
            {
                response.ErrorMessage = "Permission not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Check code already exists
            var existingPermission = await _unitOfWork.Permission.GetPermissionByCodeAsync(payload.Code);
            
            if (existingPermission is not null && existingPermission.Id != request.Id)
            {
                response.ErrorMessage = "Permission code already exists.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            //Update
            permission.Code =payload.Code;
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
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message; 
            response.WithStatus(HttpStatusCode.InternalServerError); 
        }
        return response;
    }
}