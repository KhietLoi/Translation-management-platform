using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValues;

public class GetTranslationValuesHandler : IRequestHandler<GetTranslationValuesQuery, GetTranslationValuesResponse>
{
    private readonly ILogger<GetTranslationValuesHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationValuesHandler
    (
        ILogger<GetTranslationValuesHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationValuesQuery, GetTranslationValuesResponse>

    public async Task<GetTranslationValuesResponse> Handle(GetTranslationValuesQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationValuesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationValuesResponse();

        try
        {
            var query = _unitOfWork.TranslationValue
                .GetAll()
                .AsNoTracking()
                .Include(x => x.Language)
                .Include(x => x.TranslationKey)
                .ThenInclude(x => x.Namespace)
                .AsQueryable();
            if (request.TranslationKeyId.HasValue)
            {
                query = query.Where(x => x.TranslationKeyId == request.TranslationKeyId.Value);
            }

            if (request.NamespaceId.HasValue)
            {
                query = query.Where(x => x.TranslationKey.NamespaceId == request.NamespaceId.Value);
            }

            if (request.LanguageId.HasValue)
            {
                query = query.Where(x => x.LanguageId == request.LanguageId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            var entities = await query
                .OrderBy(x => x.TranslationKey.Key)
                .ThenBy(x => x.Language.Code)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetTranslationValuesResult
            {
                TranslationValues = entities.Select(x => new GetTranslationValuesData
                {
                    Id = x.Id,
                    TranslationKeyId = x.TranslationKeyId,
                    TranslationKey = x.TranslationKey.Key,
                    NamespaceId = x.TranslationKey.NamespaceId,
                    NamespaceName = x.TranslationKey.Namespace.Name,
                    LanguageId = x.LanguageId,
                    LanguageCode = x.Language.Code,
                    Value = x.Value,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).ToList()
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