using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.TranslationManagement.Commands.UpdateTranslationKey;

public class UpdateTranslationKeyHandler : IRequestHandler<UpdateTranslationKeyCommand, UpdateTranslationKeyResponse>
{
    private readonly ILogger<UpdateTranslationKeyHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateTranslationKeyHandler
    (
        ILogger<UpdateTranslationKeyHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
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
            var translationKey = await _unitOfWork.TranslationKey.GetByIdAsync(request.TranslationKeyId);
            if (translationKey == null)
            {
                response.ErrorMessage = "Translation key not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
    
            var exists = await _unitOfWork.TranslationKey.ExistsAsync
            (
                translationKey.ProjectId,
                translationKey.NamespaceId,
                payload.Key,
                translationKey.Id
            );
            if (exists)
            {
                response.ErrorMessage = "Translation key already exists.";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            var now = DateTime.UtcNow;
            translationKey.Key = payload.Key;
            translationKey.Description = payload.Description;
            translationKey.UpdatedAt = now;
            
            //_unitOfWork.TranslationKey.Update(translationKey);
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