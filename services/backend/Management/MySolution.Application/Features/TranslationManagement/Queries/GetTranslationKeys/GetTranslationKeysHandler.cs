using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;


namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationKeys;

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
            // var translationKeys = await _unitOfWork.TranslationKey.GetAsync(request.ProjectId,request.NamespaceId, request.Keyword);
            
            IQueryable<TranslationKey> query = _unitOfWork.TranslationKey
                .GetAll()
                .AsNoTracking()
                .Include(x => x.Namespace)
                .Include(x => x.Project)
                .AsQueryable();
                
            if (request.ProjectId.HasValue)
            {
                query = query.Where(x => x.ProjectId == request.ProjectId.Value);
            }

            if (request.NamespaceId.HasValue)
            {
                query = query.Where(x => x.NamespaceId == request.NamespaceId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                // var keywordPattern = $"%{request.Keyword}%";
                query = query.Where(x => x.Key.ToLower().Contains(request.Keyword.ToLower()));
            }
            
            var translationKeys = await query
                .OrderBy(x => x.Key)
                .ToListAsync(cancellationToken);
            
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