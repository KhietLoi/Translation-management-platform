using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetPendingNamespaceCounts;

public class GetPendingNamespaceCountsHandler : IRequestHandler<GetPendingNamespaceCountsQuery, GetPendingNamespaceCountsResponse>
{
    private readonly ILogger<GetPendingNamespaceCountsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetPendingNamespaceCountsHandler
    (
        ILogger<GetPendingNamespaceCountsHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in GetPendingNamespaceCountsQuery, GetPendingNamespaceCountsResponse>

    public async Task<GetPendingNamespaceCountsResponse> Handle(GetPendingNamespaceCountsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetPendingNamespaceCountsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetPendingNamespaceCountsResponse();

        try
        {
            var projectQuery = _unitOfWork.Project
                .GetAll()
                .AsNoTracking()
                .Where(x => x.Id == request.ProjectId);

            if (_currentUser.Roles.Contains(RoleConstants.Translator) ||
                _currentUser.Roles.Contains(RoleConstants.Reviewer))
            {
                projectQuery = projectQuery.Where(x =>
                    x.ProjectMembers.Any(pm => pm.UserId == _currentUser.UserId));
            }

            var projectExists = await projectQuery.AnyAsync(cancellationToken);
            if (!projectExists)
            {
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var namespaces =
                await _unitOfWork.TranslationValue.GetPendingNamespaceCountsAsync(request.ProjectId, cancellationToken);

            response.Data = new GetPendingNamespaceCountsResult { Namespaces = namespaces };
            
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.Created);
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