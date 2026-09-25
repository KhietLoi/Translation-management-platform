using Microsoft.EntityFrameworkCore;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Infrastructure.Services;

public class ApplicationAccessService : IApplicationAccessService
{
    private readonly IApiKeyContextAccessor  _apiKeyContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationAccessService(IApiKeyContextAccessor apiKeyContextAccessor, IUnitOfWork unitOfWork)
    {
        _apiKeyContextAccessor = apiKeyContextAccessor;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<bool> CanAccessProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var application = await GetApplicationAsync(cancellationToken);
        if (application == null)
        {
            return false;
        }

        return application.ProjectId == projectId;
    }

    public async Task<Domain.Entities.Application?> GetApplicationAsync(CancellationToken cancellationToken = default)
    {
        var apiKeyContext = _apiKeyContextAccessor.Current;
        if (apiKeyContext == null)
        {
            return null;
        }
        
        var application = await _unitOfWork.Application
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == apiKeyContext.ApplicationId, cancellationToken);
        if (application == null)
        {
            return null;
        }

        if (!application.IsActive)
        {
            return null;
        }
        
        return application;
    }
}