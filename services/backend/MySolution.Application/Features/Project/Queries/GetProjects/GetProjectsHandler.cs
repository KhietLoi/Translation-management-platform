using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Project.Queries.GetProjects;

public class GetProjectsHandler : IRequestHandler<GetProjectsQuery, GetProjectsResponse>
{
    private readonly ILogger<GetProjectsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetProjectsHandler
    (
        ILogger<GetProjectsHandler> logger,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<GetProjectsResponse> Handle
    (
        GetProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        var functionName = $"{nameof(GetProjectsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetProjectsResponse();

        try
        {
            var query = _unitOfWork.Project.GetAll().AsNoTracking();
            if (_currentUser.Roles.Contains(RoleConstants.Translator) || _currentUser.Roles.Contains(RoleConstants.Reviewer))
            {
                query = query.Where(x => x.ProjectMembers.Any(pm => pm.UserId == _currentUser.UserId));
            }
            
            var projects = await query
                .Select(x => new GetProjectData
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,

                    LanguageCount = x.ProjectLanguages.Count(),
                    MemberCount = x.ProjectMembers.Count(),
                    NamespaceCount = x.ProjectNamespaces.Count(),

                    TotalTranslationCount =
                        x.ProjectNamespaces
                            .SelectMany(n => n.TranslationKeys)
                            .SelectMany(k => k.TranslationValues)
                            .Count(),

                    CompletedTranslationCount =
                        x.ProjectNamespaces
                            .SelectMany(n => n.TranslationKeys)
                            .SelectMany(k => k.TranslationValues)
                            .Count(v => v.Status == TranslationStatus.Published),

                    ProgressPercentage =
                        !x.ProjectNamespaces
                            .SelectMany(n => n.TranslationKeys)
                            .SelectMany(k => k.TranslationValues).Any()
                            ? 0
                            :
                            (
                                x.ProjectNamespaces
                                    .SelectMany(n => n.TranslationKeys)
                                    .SelectMany(k => k.TranslationValues)
                                    .Count(v =>
                                        v.Status == TranslationStatus.Reviewed ||
                                        v.Status == TranslationStatus.Published)
                                * 100m
                            )
                            /
                            x.ProjectNamespaces
                                .SelectMany(n => n.TranslationKeys)
                                .SelectMany(k => k.TranslationValues)
                                .Count(),

                    Languages = x.ProjectLanguages
                        .Take(3)
                        .Select(pl => new ProjectLanguageItem
                        {
                            Id = pl.Language.Id,
                            Code = pl.Language.Code
                        })
                        .ToList(),

                    Members = x.ProjectMembers
                        .Take(3)
                        .Select(pm => new ProjectMemberItem
                        {
                            Id = pm.User.Id,
                            UserName = pm.User.Username
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);
            
            response.Data = new GetProjectsResult
            {
                Projects = projects
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
}