using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationKey.Commands.DeleteTranslationKey;

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
            var entity = await _unitOfWork.TranslationKey.GetByIdAsync(request.Id);
            if (entity is null)
            {
                response.ErrorMessage =
                    "Translation key not found.";

                response.WithStatus(HttpStatusCode.NotFound);

                return response;
            }

            _unitOfWork
                .TranslationKey
                .Delete(entity);

            await _unitOfWork
                .SaveAsync(cancellationToken);
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception exception)
        {
            exception.LogError(_logger, functionName);
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}