using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Language.Commands.DeleteLanguage;

public class DeleteLanguageHandler : IRequestHandler<DeleteLanguageCommand, DeleteLanguageResponse>
{
    private readonly ILogger<DeleteLanguageHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public DeleteLanguageHandler
    (
        ILogger<DeleteLanguageHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in DeleteLanguageCommand, DeleteLanguageResponse>

    public async Task<DeleteLanguageResponse> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteLanguageHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteLanguageResponse();

        try
        {
            var language = await _unitOfWork.Language
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.LanguageId, cancellationToken);
            if (language is null)
            {
                _logger.LogError($"Language with id {request.LanguageId} not found.");
                
                response.ErrorMessage = "Language not found.";
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            
            _unitOfWork.Language.Delete(language);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new DeleteLanguageData
            {
                LanguageId = language.Id,
                Code = language.Code,
                Name = language.Name,
            };
            
            _logger.LogInformation("Language deleted successfully. LanguageId = {LanguageId}", language.Id);
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