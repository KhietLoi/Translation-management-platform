using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationKeys;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeys;

public class GetTranslationKeysHandler : IRequestHandler<GetTranslationKeysQuery, GetTranslationKeysResponse>
{
    private readonly ILogger<GetTranslationKeysHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationKeysHandler
    (
        ILogger<GetTranslationKeysHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationKeysQuery, GetTranslationKeysResponse>

    public async Task<GetTranslationKeysResponse> Handle(GetTranslationKeysQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationKeysHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationKeysResponse();

        try
        {
            var translationKeys = await _unitOfWork.TranslationKey
                .GetAsync(request.ProjectId,request.NamespaceId, request.Keyword);

            response.Data = new GetTranslationKeysData
            {
                Items = translationKeys
                    .Select(x => new TranslationKeyItem
                    {
                        Id = x.Id,
                        Key = x.Key,
                        CreatedAt = x.CreatedAt,
                        Description = x.Description,
                        NamespaceId = x.NamespaceId,
                        NamespaceName = x.Namespace.Name,
                        ProjectId = x.ProjectId,
                        ProjectName = x.Project.Name
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