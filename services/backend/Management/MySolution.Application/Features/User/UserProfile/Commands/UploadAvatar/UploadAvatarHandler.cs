using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.User.UserProfile.Commands.UploadAvatar;

public class UploadAvatarHandler : IRequestHandler<UploadAvatarCommand, UploadAvatarResponse>
{
    private readonly ILogger<UploadAvatarHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureBlobService _azureBlobService;

    public UploadAvatarHandler
    (
        ILogger<UploadAvatarHandler> logger,
		IUnitOfWork unitOfWork,
        IAzureBlobService azureBlobService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _azureBlobService = azureBlobService;
    }

    #region Implementation of IRequestHandler<in UploadAvatarCommand, UploadAvatarResponse>

    public async Task<UploadAvatarResponse> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UploadAvatarHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UploadAvatarResponse();

        try
        {
            var profile =  await _unitOfWork.UserProfile.GetByIdAsync(request.UserId);
            if (profile == null)
            {
                response.ErrorMessage = "Profile not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var file = payload.AvatarFile;
            var extension = Path.GetExtension(file.FileName);
            var blobName = $"avatars/{request.UserId}/{Guid.CreateVersion7()}{extension}";
            if (!string.IsNullOrWhiteSpace(profile.AvatarBlobName))
            {
                await _azureBlobService.DeleteFileAsync(profile.AvatarBlobName,cancellationToken);
            }
            
            await using var stream = file.OpenReadStream();
            
            var uploadBlobName = await _azureBlobService.UploadFileAsync(stream,blobName, cancellationToken);
            
            profile.AvatarBlobName = uploadBlobName;
            profile.UpdatedAt =  DateTime.UtcNow;

            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new UploadAvatarData
            {
                AvatarBlobName = profile.AvatarBlobName,
                UserId = profile.UserId,
                AvatarUrl = _azureBlobService.GetFileUrl(profile.AvatarBlobName)
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