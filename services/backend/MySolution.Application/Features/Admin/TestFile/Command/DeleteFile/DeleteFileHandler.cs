using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;

namespace MySolution.Application.Features.Admin.TestFile.Command.DeleteFile;

public class DeleteFileHandler : IRequestHandler<DeleteFileCommand, DeleteFileResponse>
{
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<DeleteFileHandler> _logger;

    public DeleteFileHandler
    (
        ILogger<DeleteFileHandler> logger,
        IAzureBlobService azureBlobService
    )
    {
        _logger = logger;
        _azureBlobService = azureBlobService;
    }

    #region Implementation of IRequestHandler<in DeleteFileCommand, DeleteFileResponse>

    public async Task<DeleteFileResponse> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteFileHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteFileResponse();

        try
        {
            var deleted = await _azureBlobService.DeleteFileAsync(
                request.Payload.FileName,
                cancellationToken);
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