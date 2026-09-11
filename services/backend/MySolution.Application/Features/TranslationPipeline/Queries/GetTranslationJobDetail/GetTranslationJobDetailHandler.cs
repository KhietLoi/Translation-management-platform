using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJobDetail;

public class GetTranslationJobDetailHandler : IRequestHandler<GetTranslationJobDetailQuery, GetTranslationJobDetailResponse>
{
    private readonly ILogger<GetTranslationJobDetailHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationJobDetailHandler
    (
        ILogger<GetTranslationJobDetailHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationJobDetailQuery, GetTranslationJobDetailResponse>

    public async Task<GetTranslationJobDetailResponse> Handle(GetTranslationJobDetailQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationJobDetailHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationJobDetailResponse();

        try
        {

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