using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Domain.Enums;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationManagement.Commands.SubmitTranslation;

public class SubmitTranslationHandler : IRequestHandler<SubmitTranslationCommand, SubmitTranslationResponse>
{
    private readonly ILogger<SubmitTranslationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogService _auditLogService;

    public SubmitTranslationHandler
    (
        ILogger<SubmitTranslationHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IAuditLogService auditLogService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    #region Implementation of IRequestHandler<in SubmitTranslationCommand, SubmitTranslationResponse>

    public async Task<SubmitTranslationResponse> Handle(SubmitTranslationCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(SubmitTranslationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new SubmitTranslationResponse();

        try
        {
            var entity = await _unitOfWork.TranslationValue
                .GetAll()
                .Include(x => x.TranslationKey)
                .Include(x => x.Language)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null)
            {
                _logger.LogInformation($"{functionName} Translation value not found for Id: {request.Id}");
                
                response.ErrorMessage = "Translation value not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            if (entity.Status != TranslationStatus.Draft && entity.Status != TranslationStatus.Missing && entity.Status != TranslationStatus.Rejected)
            {
                _logger.LogInformation($"{functionName} Translation with Id {request.Id} is not in a submittable state.");
                
                response.ErrorMessage = "Only Draft, Missing, or Rejected translation can be submitted";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var oldValue = new
            {
                entity.Status
            };

            entity.Status = TranslationStatus.Translated;
            entity.TranslatedAt = DateTime.UtcNow;
            entity.TranslatedBy = _currentUser.UserId;
            
            var newValue = new
            {
                entity.Status
            };

            await _auditLogService.CreateAsync(
                _currentUser.UserId,
                AuditAction.SubmitTranslation,
                AuditConstants.TranslationValue,
                entity.Id,
                entity.TranslationKey.ProjectId,
                oldValue,
                newValue);
            
            response.Data = new SubmitTranslationData
            {
                Id = entity.Id,
                LanguageId = entity.LanguageId,
                Value = entity.Value,
                Status = entity.Status,
                TranslatedBy = entity.TranslatedBy,
                TranslatedAt = entity.TranslatedAt,
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