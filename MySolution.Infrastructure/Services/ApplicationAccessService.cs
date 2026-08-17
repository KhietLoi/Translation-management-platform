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
        var apiKeyContext = _apiKeyContextAccessor.Current;
        if (apiKeyContext == null)
        {
            return false;
        }

        var application = await _unitOfWork.Application.GetByIdAsync(apiKeyContext.ApplicationId, cancellationToken);
        if (application == null)
        {
            return false;
        }

        if (!application.IsActive)
        {
            return false;
        }
        
        return application.ProjectId == projectId;
    }
}