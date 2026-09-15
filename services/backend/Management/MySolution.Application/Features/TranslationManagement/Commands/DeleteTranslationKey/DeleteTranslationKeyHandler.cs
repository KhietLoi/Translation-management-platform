using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.TranslationManagement.Commands.DeleteTranslationKey;

public class DeleteTranslationKeyHandler : IRequestHandler<DeleteTranslationKeyCommand, DeleteTranslationKeyResponse>
{
    private readonly ILogger<DeleteTranslationKeyHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public DeleteTranslationKeyHandler
    (
        ILogger<DeleteTranslationKeyHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in DeleteTranslationKeyCommand, DeleteTranslationKeyResponse>

    public async Task<DeleteTranslationKeyResponse> Handle(DeleteTranslationKeyCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteTranslationKeyHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteTranslationKeyResponse();

        try
        {
            var entity = await _unitOfWork.TranslationKey
                .GetAll()
                .AnyAsync(x => x.Id == request.Id, cancellationToken);
            if (!entity)
            {
                _logger.LogError($"No translation with id {request.Id} found.");
                
                response.ErrorMessage = "Translation key not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            _unitOfWork.TranslationKey.Delete(new Domain.Entities.TranslationKey { Id = request.Id });

            await _unitOfWork.SaveAsync(cancellationToken);
            
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