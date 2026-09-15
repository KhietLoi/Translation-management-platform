using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.TranslationManagement.Commands.DeleteTranslationValue;

public class DeleteTranslationValueHandler : IRequestHandler<DeleteTranslationValueCommand, DeleteTranslationValueResponse>
{
    private readonly ILogger<DeleteTranslationValueHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public DeleteTranslationValueHandler
    (
        ILogger<DeleteTranslationValueHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in DeleteTranslationValueCommand, DeleteTranslationValueResponse>

    public async Task<DeleteTranslationValueResponse> Handle(DeleteTranslationValueCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteTranslationValueHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteTranslationValueResponse();

        try
        {
            var affectedRows = await _unitOfWork.TranslationValue
                .GetAll()
                .Where(x => x.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            var deleted = affectedRows > 0;
            if (!deleted)
            {
                _logger.LogInformation("{FunctionName} TranslationValue not found for Id: {Id}", functionName, request.Id);
                
                response.ErrorMessage = "TranslationValue not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
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