using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDetail;

public class GetReleaseDetailHandler : IRequestHandler<GetReleaseDetailQuery, GetReleaseDetailResponse>
{
    private readonly ILogger<GetReleaseDetailHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetReleaseDetailHandler
    (
        ILogger<GetReleaseDetailHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetReleaseDetailQuery, GetReleaseDetailResponse>

    public async Task<GetReleaseDetailResponse> Handle(GetReleaseDetailQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetReleaseDetailHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetReleaseDetailResponse();

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