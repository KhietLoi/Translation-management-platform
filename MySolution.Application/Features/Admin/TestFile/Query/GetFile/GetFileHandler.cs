using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;

namespace MySolution.Application.Features.TestFile.Query.GetFile;

public class GetFileHandler : IRequestHandler<GetFileQuery, GetFileResponse>
{
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<GetFileHandler> _logger;

    public GetFileHandler(IAzureBlobService azureBlobService, ILogger<GetFileHandler> logger)
    {
        _azureBlobService = azureBlobService;
        _logger = logger;
    }

    public async Task<GetFileResponse> Handle(
        GetFileQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetFileResponse();

        try
        {
            response.Files = await _azureBlobService.GetAllFilesAsync(cancellationToken);
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Handler} Unexpected error.", nameof(GetFileHandler));
            response.ErrorMessage = "An unexpected error occurred.";
            response
                .WithSuccess(false)
                .WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }
}