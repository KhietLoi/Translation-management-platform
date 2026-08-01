using System.Net;
using MediatR;
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
            var (items, totalCount) =
                await _unitOfWork.TranslationKey.GetByGridAsync
                (
                    request.ProjectId,
                    request.NamespaceId,
                    request.Keyword,
                    request.Status,
                    request.PageNumber,
                    request.PageSize
                );
            
            response.Data = new GetTranslationGridData
            {
                TotalCount = totalCount,
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