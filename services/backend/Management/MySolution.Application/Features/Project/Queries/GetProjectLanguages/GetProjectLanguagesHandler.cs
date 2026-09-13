using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Project.Queries.GetProjectLanguages;

public class GetProjectLanguagesHandler : IRequestHandler<GetProjectLanguagesQuery, GetProjectLanguagesResponse>
{
    private readonly ILogger<GetProjectLanguagesHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetProjectLanguagesHandler
    (
        ILogger<GetProjectLanguagesHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetProjectLanguagesQuery, GetProjectLanguagesResponse>

    public async Task<GetProjectLanguagesResponse> Handle(GetProjectLanguagesQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetProjectLanguagesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetProjectLanguagesResponse();

        try
        {
            //Check project is already exists
            var isProject = await _unitOfWork.Project
                .GetAll()
                .AnyAsync(x => x.Id == request.ProjectId, cancellationToken);
            if (!isProject)
            {
                _logger.LogInformation("{FunctionName} Project not found. ProjectId: {ProjectId}", functionName, request.ProjectId);
                
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var languages = await _unitOfWork.ProjectLanguage
                .GetAll()
                .AsNoTracking()
                .Include(x => x.Language)
                .Where(x => x.ProjectId == request.ProjectId)
                .ToListAsync(cancellationToken);
            
            response.Data =  new GetProjectLanguagesResult
            {
                Languages = languages.Select(x =>
                    new GetProjectLanguageData
                    {
                        LanguageId = x.LanguageId,
                        Code = x.Language.Code,
                        Name = x.Language.Name
                    }).ToList()
            };

            _logger.LogInformation("{FunctionName} Successfully retrieved project languages. ProjectId: {ProjectId}," +
                                   " LanguagesCount: {LanguagesCount}", functionName, request.ProjectId, response.Data.Languages.Count);
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