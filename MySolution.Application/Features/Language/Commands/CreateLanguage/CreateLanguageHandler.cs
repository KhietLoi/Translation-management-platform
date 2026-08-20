using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Language.Commands.CreateLanguage;

public class CreateLanguageHandler : IRequestHandler<CreateLanguageCommand, CreateLanguageResponse>
{
    private readonly ILogger<CreateLanguageHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateLanguageHandler
    (
        ILogger<CreateLanguageHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateLanguageCommand, CreateLanguageResponse>

    public async Task<CreateLanguageResponse> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var payload =  request.Payload;
        var functionName = $"{nameof(CreateLanguageHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateLanguageResponse();

        try
        {
            var isCodeExits = await _unitOfWork.Language.ExistsByCodeAsync(payload.Code);
            if (isCodeExits)
            {
                response.ErrorMessage = "Code already exists";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }

            var language = new Domain.Entities.Language
            {
                Id = Guid.CreateVersion7(),
                Code = payload.Code,
                Name = payload.Name,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Language.Add(language);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new CreateLanguageData
            {
                Id = language.Id,
                Name = language.Name,
                Code = language.Code,
                CreatedAt = language.CreatedAt
            };

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