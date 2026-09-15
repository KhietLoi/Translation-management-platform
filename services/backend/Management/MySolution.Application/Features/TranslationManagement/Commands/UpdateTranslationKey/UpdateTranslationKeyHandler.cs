using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.UpdateTranslationKey;

public class UpdateTranslationKeyHandler : IRequestHandler<UpdateTranslationKeyCommand, UpdateTranslationKeyResponse>
{
    private readonly ILogger<UpdateTranslationKeyHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService  _auditLogService;
    private readonly ICurrentUser _currentUser;

    public UpdateTranslationKeyHandler
    (
        ILogger<UpdateTranslationKeyHandler> logger,
		IUnitOfWork unitOfWork,
        IAuditLogService auditLogService,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _auditLogService  = auditLogService;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in UpdateTranslationKeyCommand, UpdateTranslationKeyResponse>

    public async Task<UpdateTranslationKeyResponse> Handle(UpdateTranslationKeyCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateTranslationKeyHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateTranslationKeyResponse();

        try
        {
            var translationKey = await _unitOfWork.TranslationKey
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.TranslationKeyId, cancellationToken);
            if (translationKey == null)
            {
                _logger.LogInformation("{FunctionName} Translation key not found. TranslationKeyId: {TranslationKeyId}", functionName, request.TranslationKeyId);
                
                response.ErrorMessage = "Translation key not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var exists = await _unitOfWork.TranslationKey
                .GetAll()
                .AnyAsync(x => 
                    x.ProjectId == translationKey.ProjectId &&
                    x.NamespaceId == translationKey.NamespaceId &&
                    x.Key == payload.Key &&
                    x.Id != translationKey.Id, cancellationToken);
            
            if (exists)
            {
                _logger.LogInformation("{FunctionName} Translation key already exists.", functionName);
                
                response.ErrorMessage = "Translation key already exists.";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            object oldValue = new
            {
               translationKey.Key,
               translationKey.Description
            };
            
            var now = DateTime.UtcNow;
            translationKey.Description = payload.Description;
            translationKey.UpdatedAt = now;
            translationKey.Key = payload.Key;

            object newValue = new
            {
                payload.Key,
                payload.Description
            };
            
            //Audit log:
            await _auditLogService.CreateAsync(
                _currentUser.UserId,
                AuditAction.Update,
                translationKey.Key,
                translationKey.Id,
                translationKey.ProjectId,
                oldValue,
                newValue
            );
            
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new UpdateTranslationKeyData
            {
                Id = translationKey.Id,
                ProjectId = translationKey.ProjectId,
                NamespaceId = translationKey.NamespaceId,
                Key = payload.Key,
                Description = payload.Description,
                CreatedAt = translationKey.CreatedAt,
                UpdatedAt = now
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

    #endregion
}