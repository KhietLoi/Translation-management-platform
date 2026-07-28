using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Language.Commands.UpdateLanguage;

public class UpdateLanguageHandler : IRequestHandler<UpdateLanguageCommand, UpdateLanguageResponse>
{
    private readonly ILogger<UpdateLanguageHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateLanguageHandler
    (
        ILogger<UpdateLanguageHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateLanguageCommand, UpdateLanguageResponse>

    public async Task<UpdateLanguageResponse> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateLanguageHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateLanguageResponse();

        try
        {
            var language = await _unitOfWork.Language.GetByIdAsync(request.LanguageId);
            if (language == null)
            {
                response.ErrorMessage = "Language not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Check if code of language already exists
            var isCodeExist = await _unitOfWork.Language.ExistsByCodeAsync(payload.Code, request.LanguageId);
            if (isCodeExist)
            {
                response.ErrorMessage = "Code already exists";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            language.Code = payload.Code;
            language.Name = payload.Name;
            language.UpdatedAt = DateTime.UtcNow;
  
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new UpdateLanguageData
            {
                LanguageId = language.Id,
                Name = language.Name,
                Code = language.Code,
                CreatedAt = language.CreatedAt,
                UpdatedAt = DateTime.UtcNow
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