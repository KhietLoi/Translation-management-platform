using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDiff;

public class GetReleaseDiffHandler : IRequestHandler<GetReleaseDiffQuery, GetReleaseDiffResponse>
{
    private readonly ILogger<GetReleaseDiffHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IReleaseDiffService _releaseDiffService;

    public GetReleaseDiffHandler
    (
        ILogger<GetReleaseDiffHandler> logger,
		IUnitOfWork unitOfWork,
        IReleaseDiffService releaseDiffService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _releaseDiffService = releaseDiffService;
    }

    #region Implementation of IRequestHandler<in GetReleaseDiffQuery, GetReleaseDiffResponse>

    public async Task<GetReleaseDiffResponse> Handle(GetReleaseDiffQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetReleaseDiffHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetReleaseDiffResponse();

        try
        {
            var sourceRelease = await _unitOfWork.TranslationRelease.GetByIdAsync(request.SourceReleaseId, cancellationToken);
            if (sourceRelease == null)
            {
                response.ErrorMessage = "The source release was not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var targetRelease = await _unitOfWork.TranslationRelease.GetByIdAsync(request.TargetReleaseId, cancellationToken);
            if (targetRelease == null)
            {
                response.ErrorMessage = "The target release was not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            if (sourceRelease.ProjectId != targetRelease.ProjectId)
            {
                response.ErrorMessage = "Releases must belong to the same project.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var result = await _releaseDiffService.CompareAsync(sourceRelease, targetRelease, cancellationToken);
            
            response.Data =
                new GetReleaseDiffData
                {
                    AddedCount = result.AddedCount,
                    UpdatedCount = result.UpdatedCount,
                    RemovedCount = result.RemovedCount,

                    Added = result.Added
                        .Select(x => new GetReleaseDiffItem
                        {
                            LanguageCode = x.LanguageCode,
                            Key = x.Key,
                            OldValue = x.OldValue,
                            NewValue = x.NewValue
                        }).ToList(),

                    Updated = result.Updated
                        .Select(x => new GetReleaseDiffItem
                        {
                            LanguageCode = x.LanguageCode,
                            Key = x.Key,
                            OldValue = x.OldValue,
                            NewValue = x.NewValue
                        }).ToList(),

                    Removed = result.Removed
                        .Select(x => new GetReleaseDiffItem
                        {
                            LanguageCode = x.LanguageCode,
                            Key = x.Key,
                            OldValue = x.OldValue,
                            NewValue = x.NewValue
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