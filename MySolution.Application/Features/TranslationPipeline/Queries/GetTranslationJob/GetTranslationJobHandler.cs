using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
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
            var project = await _unitOfWork.Project.ExistsAsync(request.ProjectId);
            if (!project)
            {
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var (items, totalCount) =
                await _unitOfWork.TranslationJob
                    .GetHistoryAsync(request.ProjectId, request.PageNumber, request.PageSize, cancellationToken);
            
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
                        CompletedAt = x.CreatedAt
                    }).ToList(),

                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
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