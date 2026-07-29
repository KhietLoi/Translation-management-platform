using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationKey.Commands.CreateTranslationKey;

public class CreateTranslationKeyHandler : IRequestHandler<CreateTranslationKeyCommand, CreateTranslationKeyResponse>
{
    private readonly ILogger<CreateTranslationKeyHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateTranslationKeyHandler
    (
        ILogger<CreateTranslationKeyHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateTranslationKeyCommand, CreateTranslationKeyResponse>

    public async Task<CreateTranslationKeyResponse> Handle(CreateTranslationKeyCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateTranslationKeyHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateTranslationKeyResponse();

        try
        {
            //Check TranslationKey is already exists
            var exists = await _unitOfWork.TranslationKey.ExistsAsync(payload.ProjectId, payload.NamespaceId, payload.Key);
            if (exists)
            {
                response.ErrorMessage = "Translation key already exists.";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            var isValidNamespace = await _unitOfWork.Namespace.IsNamespaceBelongsToProjectAsync(payload.NamespaceId, payload.ProjectId);
            if (!isValidNamespace)
            {
                response.ErrorMessage = "Namespace does not belong to the specified project.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var entity = new Domain.Entities.TranslationKey
            {
                Id = Guid.CreateVersion7(),
                ProjectId = payload.ProjectId,
                NamespaceId = payload.NamespaceId,
                Key = payload.Key,
                Description = payload.Description,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.TranslationKey.Add(entity);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new CreateTranslationKeyData
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                NamespaceId = entity.NamespaceId,
                Key = entity.Key,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
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

    #endregion
}