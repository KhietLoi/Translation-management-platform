using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IProjectLanguageRepository : IRepository<ProjectLanguage>
{
    Task<List<ProjectLanguage>> GetByProjectIdAsync(Guid projectId);
    Task<List<ProjectLanguage>> GetByProjectIdsAsync(List<Guid> projectIds);
    Task<List<ProjectLanguage>> GetByProjectIdWithLanguageAsync(Guid projectId);
    Task<bool> IsLanguageBelongsToProjectAsync(Guid languageId, Guid projectId);
    
    // Get all languages by project id
    Task<List<Language>> GetLanguagesByProjectIdAsync(Guid projectId);
}