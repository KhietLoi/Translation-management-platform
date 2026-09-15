using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJob;

public class GetTranslationJobHandler : IRequestHandler<GetTranslationJobQuery, GetTranslationJobResponse>
{
    private readonly ILogger<GetTranslationJobHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationJobHandler
    (
        ILogger<GetTranslationJobHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationJobQuery, GetTranslationJobResponse>

    public async Task<GetTranslationJobResponse> Handle(GetTranslationJobQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationJobHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationJobResponse();

        try
        {
            //Check projectId:
            var project = await _unitOfWork.Project
                .GetAll()
                .AsNoTracking()
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);
            if (!project)
            {
                _logger.LogInformation("{FunctionName} Project not found. ProjectId = {ProjectId}", functionName, request.ProjectId);
                
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var query = _unitOfWork.TranslationJob
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ProjectId == request.ProjectId &&
                            (x.Type == TranslationJobType.Export || x.Type == TranslationJobType.Import));
            
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetTranslationJobResponseData
            {
                Items = items
                    .Select(x => new GetTranslationJobHistoryItem
                    {
                        Id = x.Id,
                        ProjectId = x.ProjectId,
                        LanguageId = x.LanguageId,
                        NamespaceId = x.NamespaceId,
                        TranslationJobType = x.Type,
                        Status = x.Status,
                        FileType = x.FileType,
                        FileName = x.FileName,
                        DownloadUrl = x.DownloadUrl,
                        CompletedAt = x.CreatedAt,
                        SuccessRecords = x.SuccessRecords,
                        FailedRecords = x.FailedRecords,
                        TotalRecords = x.TotalRecords,
                        SkipRecords = x.SkippedRecords
                    }).ToList(),

                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            
            _logger.LogInformation("{FunctionName} Translation jobs retrieved successfully. ProjectId = {ProjectId}", functionName, request.ProjectId);
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