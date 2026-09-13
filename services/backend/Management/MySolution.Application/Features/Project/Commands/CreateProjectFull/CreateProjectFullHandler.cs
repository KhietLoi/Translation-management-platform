using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Project.Commands.CreateProjectFull;

public class CreateProjectFullHandler : IRequestHandler<CreateProjectFullCommand, CreateProjectFullResponse>
{
    private readonly ILogger<CreateProjectFullHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateProjectFullHandler
    (
        ILogger<CreateProjectFullHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateProjectFullCommand, CreateProjectFullResponse>

    public async Task<CreateProjectFullResponse> Handle(CreateProjectFullCommand request, CancellationToken cancellationToken)
    {
        var payload =  request.Payload;
        var functionName = $"{nameof(CreateProjectFullHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateProjectFullResponse();

        try
        {
            
            // Check project name already exists
            var isNameExists = await _unitOfWork.Project.ExistsByNameAsync(payload.Name);
            if (isNameExists)
            {
                _logger.LogInformation("{FunctionName} Project with name {ProjectName} already exists", functionName, payload.Name);
                
                response.ErrorMessage = $"Project with name {payload.Name} already exists";
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            var project = new Domain.Entities.Project
            {
                Id =  Guid.CreateVersion7(),
                Name = request.Payload.Name,
                Description = request.Payload.Description,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.Project.Add(project);
            
            // Check Language Ids are valid
            var languageIds = payload.LanguageIds.Distinct().ToList();
            var existingLanguages = await _unitOfWork.Language
                .GetAll()
                .Where(x => languageIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
            
            var existingLanguageIds = existingLanguages.Select(x => x.Id).ToHashSet();
            var invalidLanguageIds = languageIds.Except(existingLanguageIds).ToList();
            if (invalidLanguageIds.Any())
            {
                response.ErrorMessage = "Invalid Language Ids";
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var projectLanguages =
                languageIds
                    .Select(languageId =>
                        new Domain.Entities.ProjectLanguage
                        {
                            ProjectId = project.Id,
                            LanguageId = languageId,
                            CreatedAt = DateTime.UtcNow
                        })
                    .ToList();
            
            if (projectLanguages.Count > 0)
            {
                await _unitOfWork.ProjectLanguage.AddRange(projectLanguages);
            }
          
            // Add members
            var projectMembers =
                payload.MemberIds
                    .Distinct()
                    .Select(userId =>
                        new Domain.Entities.ProjectMember
                        {
                            ProjectId = project.Id,
                            UserId = userId,
                            CreatedAt = DateTime.UtcNow
                        })
                    .ToList();
            
            if (projectMembers.Count > 0)
            {
                await _unitOfWork.ProjectMember.AddRange(projectMembers);
            }
            // Add Namespaces
            var namespaces =
                payload.Namespaces
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .Select(x =>
                        new Domain.Entities.ProjectNamespace
                        {
                            Id = Guid.CreateVersion7(),
                            ProjectId = project.Id,
                            Name = x.Name.Trim(),
                            CreatedAt = DateTime.UtcNow
                        })
                    .ToList();

            if (namespaces.Count > 0)
            {
                await _unitOfWork.Namespace.AddRange(namespaces);
            }

            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new CreateProjectFullData
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
            };
            
            _logger.LogInformation("{FunctionName} Project {ProjectName} created successfully with Id {ProjectId}", functionName, project.Name, project.Id);
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