using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
namespace MySolution.Application.Features.Project.Commands.UpdateProjectLanguages;

public class UpdateProjectLanguagesHandler : IRequestHandler<UpdateProjectLanguagesCommand, UpdateProjectLanguagesResponse>
{
    private readonly ILogger<UpdateProjectLanguagesHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectLanguagesHandler
    (
        ILogger<UpdateProjectLanguagesHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateProjectLanguagesCommand, UpdateProjectLanguagesResponse>

    public async Task<UpdateProjectLanguagesResponse> Handle
    (
        UpdateProjectLanguagesCommand request, 
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateProjectLanguagesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateProjectLanguagesResponse();

        try
        {
            //Check project is already exists
            var isProject = await _unitOfWork.Project.ExistsAsync(request.ProjectId);
            if (!isProject)
            {
                response.ErrorMessage = "Project not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            // Current project Languages
            var currentProjectLanguages = await _unitOfWork
                .ProjectLanguage
                .GetByProjectIdAsync(request.ProjectId);
            // Change to Hashset
            var currentProjectIds = currentProjectLanguages
                .Select(x => x.LanguageId)
                .ToHashSet();
            // New language ids
            var newLanguageIds = payload.LanguagesIds.Distinct().ToHashSet();
            // Calc Add
            var languageIdsToAdd  = newLanguageIds.Except(currentProjectIds).ToList();
            // Language to remove:
            var projectLanguagesToRemove = currentProjectLanguages
                .Where(x => !newLanguageIds
                    .Contains(x.LanguageId)).ToList();
            if (languageIdsToAdd.Count > 0)
            {
                var languages = await _unitOfWork.Language.GetByIdsAsync(languageIdsToAdd);
                var foundIds = languages.Select(x => x.Id).ToHashSet();
                var invalidIds = languageIdsToAdd.Except(foundIds).ToList();
                if (invalidIds.Count > 0)
                {
                    response.ErrorMessage = "One or more languages do not exist.";
                    response.WithStatus(HttpStatusCode.BadRequest);
                    return response;
                }

                var entities =
                    languageIdsToAdd
                        .Select(x =>
                            new ProjectLanguage
                            {
                                ProjectId = request.ProjectId,
                                LanguageId = x,
                                CreatedAt = DateTime.UtcNow
                            })
                        .ToList();

                await _unitOfWork.ProjectLanguage.AddRange(entities);
                
                var translationKeys =
                    await _unitOfWork
                        .TranslationKey
                        .GetAll()
                        .Where(x => x.ProjectId == request.ProjectId)
                        .ToListAsync(cancellationToken);
                
                var existingValues =
                    await _unitOfWork
                        .TranslationValue
                        .GetAll()
                        .Where(x =>
                            x.TranslationKey.ProjectId ==
                            request.ProjectId)
                        .Select(x => new
                        {
                            x.TranslationKeyId,
                            x.LanguageId
                        })
                        .ToListAsync(cancellationToken);
                
                var existingPairs =
                    existingValues
                        .Select(x =>
                        (
                            x.TranslationKeyId,
                            x.LanguageId
                        ))
                        .ToHashSet();

                var translationValuesToCreate =
                (
                    from key in translationKeys
                    from languageId in languageIdsToAdd
                    where !existingPairs.Contains((key.Id, languageId))
                    select new Domain.Entities.TranslationValue
                    {
                        TranslationKeyId = key.Id,
                        LanguageId = languageId,
                        Value = string.Empty,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = null
                    }
                ).ToList();

                if (translationValuesToCreate.Count > 0)
                {
                    await _unitOfWork
                        .TranslationValue
                        .AddRange(
                            translationValuesToCreate
                        );
                }
            }

            if (projectLanguagesToRemove.Count > 0)
            {
                _unitOfWork.ProjectLanguage.DeleteRange(projectLanguagesToRemove);
            }
            
            await _unitOfWork.SaveAsync(cancellationToken);
            var updatedLanguages = await _unitOfWork.Language.GetByIdsAsync(newLanguageIds.ToList());
            response.Data = new UpdateProjectLanguagesResult
            {
                ProjectId = request.ProjectId,
                Languages = updatedLanguages
                    .Select(x => new UpdateProjectLanguagesData
                    {
                        LanguageId = x.Id,
                        Code = x.Code,
                        Name = x.Name
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