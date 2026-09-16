using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationKeyById;

public class GetTranslationKeyByIdHandler : IRequestHandler<GetTranslationKeyByIdQuery, GetTranslationKeyByIdResponse>
{
    private readonly ILogger<GetTranslationKeyByIdHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationKeyByIdHandler
    (
        ILogger<GetTranslationKeyByIdHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationKeyByIdQuery, GetTranslationKeyByIdResponse>

    public async Task<GetTranslationKeyByIdResponse> Handle(GetTranslationKeyByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationKeyByIdHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationKeyByIdResponse();

        try
        {
            var translationKey = await _unitOfWork.TranslationKey
                .GetAll()
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Namespace)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
            if (translationKey == null)
            {
                _logger.LogInformation("{FunctionName} Translation key with ID {Id} not found.", functionName, request.Id);
                
                response.ErrorMessage = "Translation key not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetTranslationKeyByIdData
            {
                Id = translationKey.Id,
                Key = translationKey.Key,
                Description = translationKey.Description,
                CreatedAt = translationKey.CreatedAt,
                UpdatedAt = translationKey.UpdatedAt,
                ProjectId = translationKey.ProjectId,
                ProjectName = translationKey.Project.Name,
                NamespaceId = translationKey.NamespaceId,
                NamespaceName = translationKey.Namespace.Name
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