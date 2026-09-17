using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationGrid;

public class GetTranslationGridHandler : IRequestHandler<GetTranslationGridQuery, GetTranslationGridResponse>
{
    private readonly ILogger<GetTranslationGridHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationGridHandler
    (
        ILogger<GetTranslationGridHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationGridQuery, GetTranslationGridResponse>

    public async Task<GetTranslationGridResponse> Handle(GetTranslationGridQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationGridHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationGridResponse();

        try
        {
            var query = _unitOfWork.TranslationKey
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ProjectId == request.ProjectId);

            if (request.NamespaceId.HasValue)
            {
                query = query.Where(x => x.NamespaceId == request.NamespaceId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x =>
                    x.Key.Contains(request.Keyword) ||
                    (x.Description != null &&
                     x.Description.Contains(request.Keyword)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x =>
                    x.TranslationValues.Any(v =>
                        v.Status == request.Status.Value));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Include(x => x.Namespace)
                .Include(x => x.TranslationValues
                    .OrderBy(v => v.Language.Code)
                    .Take(request.NumberOfLanguages))
                .ThenInclude(x => x.Language)
                .OrderBy(x => x.Key)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetTranslationGridData
            {
                TotalCount = totalCount,
                NumberOfLanguages = request.NumberOfLanguages,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            
                Items = items.Select(x =>
                        new TranslationGridItem
                        {
                            TranslationKeyId = x.Id,
                            NamespaceId = x.NamespaceId,
                            NamespaceName = x.Namespace.Name,
                            Key = x.Key,
                            Description = x.Description,

                            Values = x.TranslationValues
                                .OrderBy(v => v.Language.Code)
                                .Select(v =>
                                    new TranslationCellItem
                                    {
                                        TranslationValueId = v.Id,
                                        LanguageId = v.LanguageId,
                                        LanguageCode = v.Language.Code,
                                        Value = v.Value,
                                        Status = v.Status
                                    })
                                .ToList()
                        })
                    .ToList()
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
