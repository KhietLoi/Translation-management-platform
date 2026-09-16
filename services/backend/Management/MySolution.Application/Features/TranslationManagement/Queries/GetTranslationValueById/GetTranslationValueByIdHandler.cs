using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValueById;

public class GetTranslationValueByIdHandler : IRequestHandler<GetTranslationValueByIdQuery, GetTranslationValueByIdResponse>
{
    private readonly ILogger<GetTranslationValueByIdHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationValueByIdHandler
    (
        ILogger<GetTranslationValueByIdHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationValueByIdQuery, GetTranslationValueByIdResponse>

    public async Task<GetTranslationValueByIdResponse> Handle(GetTranslationValueByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationValueByIdHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationValueByIdResponse();

        try
        {
            var entity = await _unitOfWork.TranslationValue
                .GetAll()
                .AsNoTracking()
                .Include(x => x.TranslationKey)
                    .ThenInclude(x => x.Namespace)
                .Include(x => x.Language)
                .FirstOrDefaultAsync(x => x.Id == request.Id,cancellationToken);
            
            if (entity == null)
            {
                _logger.LogInformation($"{functionName} Translation value not found for Id: {request.Id}");
                
                response.ErrorMessage = "Translation value not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetTranslationValueByIdData
            {
                Id = entity.Id,
                TranslationKeyId = entity.TranslationKeyId,
                TranslationKey = entity.TranslationKey.Key,
                Description = entity.TranslationKey.Description,
                NamespaceId = entity.TranslationKey.NamespaceId,
                NamespaceName = entity.TranslationKey.Namespace.Name,
                LanguageId = entity.Language.Id,
                LanguageCode = entity.Language.Code,
                Value = entity.Value,
                Status = entity.Status,
                RejectionReason = entity.RejectionReason,
                TranslatedBy = entity.TranslatedBy,
                TranslatedAt = entity.TranslatedAt,
                ReviewedBy = entity.ReviewedBy,
                ReviewedAt = entity.ReviewedAt,
                PublishedBy = entity.PublishedBy,
                PublishedAt = entity.PublishedAt,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
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