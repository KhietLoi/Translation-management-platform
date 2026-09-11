using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.TranslationValue.Commands.CreateTranslationValue;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationValue;

public class CreateTranslationValueHandler : IRequestHandler<CreateTranslationValueCommand, CreateTranslationValueResponse>
{
    private readonly ILogger<CreateTranslationValueHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateTranslationValueHandler
    (
        ILogger<CreateTranslationValueHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateTranslationValueCommand, CreateTranslationValueResponse>

    public async Task<CreateTranslationValueResponse> Handle(CreateTranslationValueCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateTranslationValueHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateTranslationValueResponse();

        try
        {
            // Check TranslationKey exists
            var translationKey = await _unitOfWork.TranslationKey.ExistsAsync(payload.TranslationKeyId);
            if (!translationKey)
            {
                response.ErrorMessage = "Translation key not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            Guid projectId = await _unitOfWork.TranslationKey.GetProjectIdAsync(payload.TranslationKeyId);
            
            /*
            var isValidLanguage = await _unitOfWork.Namespace.IsNamespaceBelongsToProjectAsync(payload.LanguageId, projectId);
            if (!isValidLanguage)
            {
                response.ErrorMessage = "Language does not belong to the specified project.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }*/
            
            // Check duplicate
            var exists = await _unitOfWork.TranslationValue.ExistsAsync(payload.TranslationKeyId, payload.LanguageId);
            if (exists)
            {
                response.ErrorMessage = "Translation key already exists";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            //Add new TranslationValue:
            var entity = new Domain.Entities.TranslationValue
            {
                Id = Guid.CreateVersion7(),
                LanguageId = payload.LanguageId,
                TranslationKeyId = payload.TranslationKeyId,
                Value = payload.Value,
                Status = TranslationStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.TranslationValue.Add(entity);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new CreateTranslationValueData
            {
                Id = entity.Id,
                LanguageId = entity.LanguageId,
                TranslationKeyId = entity.TranslationKeyId,
                Value = entity.Value,
                Status = TranslationStatus.Draft,
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