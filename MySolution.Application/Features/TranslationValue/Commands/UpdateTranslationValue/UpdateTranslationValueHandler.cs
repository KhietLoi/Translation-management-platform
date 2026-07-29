using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationValue.Commands.UpdateTranslationValue;

public class UpdateTranslationValueHandler : IRequestHandler<UpdateTranslationValueCommand, UpdateTranslationValueResponse>
{
    private readonly ILogger<UpdateTranslationValueHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateTranslationValueHandler
    (
        ILogger<UpdateTranslationValueHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateTranslationValueCommand, UpdateTranslationValueResponse>

    public async Task<UpdateTranslationValueResponse> Handle(UpdateTranslationValueCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateTranslationValueHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateTranslationValueResponse();

        try
        {
            var entity = await _unitOfWork.TranslationValue.GetByIdTrackingAsync(request.Id);
            if (entity == null)
            {
                response.ErrorMessage = "TranslationValue not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var now = DateTime.UtcNow;
            entity.Value = payload.Value;
            entity.UpdatedAt = now;

            response.Data = new UpdateTranslationValueData
            {
                Id = entity.Id,
                TranslationKeyId = entity.TranslationKeyId,
                LanguageId = entity.LanguageId,
                Value = entity.Value,
                Status = TranslationStatus.Draft,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = now
            };
            
            await _unitOfWork.SaveAsync(cancellationToken);

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