using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Application.Constants;
using MySolution.Domain.Enums;


namespace MySolution.Application.Features.TranslationManagement.Queries.GetPendingLanguageCounts;

public class GetPendingLanguageCountsHandler : IRequestHandler<GetPendingLanguageCountsQuery, GetPendingLanguageCountsResponse>
{
    private readonly ILogger<GetPendingLanguageCountsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetPendingLanguageCountsHandler
    (
        ILogger<GetPendingLanguageCountsHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in GetPendingLanguageCountsQuery, GetPendingLanguageCountsResponse>

    public async Task<GetPendingLanguageCountsResponse> Handle(GetPendingLanguageCountsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetPendingLanguageCountsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetPendingLanguageCountsResponse();

        try
        {
           var projectQuery = _unitOfWork.Project
               .GetAll()
               .AsNoTracking()
               .Where(x => x.Id == request.ProjectId);

            if (_currentUser.Roles.Contains(RoleConstants.Translator) || _currentUser.Roles.Contains(RoleConstants.Reviewer))
            {
                projectQuery = projectQuery.Where(x => x.ProjectMembers.Any(pm => pm.UserId == _currentUser.UserId));
            }

            var projectExists = await projectQuery.AnyAsync(cancellationToken);
            if (!projectExists)
            {
                _logger.LogInformation(functionName + " project not found.");
                
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var namespaceExists = await _unitOfWork.Namespace
                 .GetAll()
                 .AsNoTracking()
                 .AnyAsync(x =>
                     x.Id == request.NamespaceId &&
                     x.ProjectId == request.ProjectId,
                 cancellationToken);

            if (!namespaceExists)
            {
                _logger.LogInformation(functionName + " namespace not found.");
                
                response.ErrorMessage = "Namespace not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var languages = await _unitOfWork.TranslationValue
                .GetAll()
                .AsNoTracking()
                .Where (x =>
                    x.TranslationKey.ProjectId == request.ProjectId &&
                    x.TranslationKey.NamespaceId == request.NamespaceId)
                .GroupBy(x => x.LanguageId)
                .Select(g => new PendingLanguageCount
                {
                    LanguageId = g.Key,
                    PendingReviewCount = g.Count(x => x.Status == TranslationStatus.Translated),
                    PendingUpdateCount = g.Count(x =>
                        x.Status == TranslationStatus.Missing ||
                        x.Status == TranslationStatus.Draft ||
                        x.Status == TranslationStatus.Rejected)
                }).ToListAsync(cancellationToken);
            
            response.Data = new GetPendingLanguageCountsResult
            {
                Languages = languages
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