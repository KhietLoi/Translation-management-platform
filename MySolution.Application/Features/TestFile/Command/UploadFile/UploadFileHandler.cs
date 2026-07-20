using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Features.User.Commands.CreateUser;

namespace MySolution.Application.Features.TestFile.Command.UploadFile;

public class UploadFileHandler : IRequestHandler<UploadFileCommand, UploadFileResponse>
{
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<UploadFileHandler> _logger;
    public UploadFileHandler(IAzureBlobService azureBlobService, ILogger<UploadFileHandler> logger)
    {
        _azureBlobService = azureBlobService;
        _logger = logger;
    }
    public async Task<UploadFileResponse> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UploadFileHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UploadFileResponse();

        try
        {
            var extension = Path.GetExtension(payload.File.FileName);
            var fileName = $"{Guid.CreateVersion7()}{extension}";
            var blobUrl = await _azureBlobService.UploadFileAsync(payload.File.OpenReadStream(), fileName, cancellationToken);
            response.BlobUrl = blobUrl;
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
}