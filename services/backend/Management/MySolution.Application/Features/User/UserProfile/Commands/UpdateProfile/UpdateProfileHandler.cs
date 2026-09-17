using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResponse>
{
    private readonly ILogger<UpdateProfileHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler
    (
        ILogger<UpdateProfileHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateProfileCommand, UpdateProfileResponse>

    public async Task<UpdateProfileResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateProfileHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateProfileResponse();

        try
        {
            var userProfile = await _unitOfWork.UserProfile
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(up => up.UserId == request.Id, cancellationToken);
            
            if (userProfile is null)
            {
                response.ErrorMessage = "Profile not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Check phone:
            var existingPhoneNumber = await _unitOfWork.UserProfile
                .GetAll()
                .AnyAsync(x => x.PhoneNumber == payload.PhoneNumber && x.UserId != request.Id, cancellationToken);            
            if (existingPhoneNumber)
            {
                response.ErrorMessage = "Phone number already exists";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            //Update:
            userProfile.PhoneNumber = payload.PhoneNumber;
            userProfile.FullName =  payload.FullName;
            userProfile.BirthDate = payload.BirthDate;
            userProfile.UpdatedAt = DateTime.UtcNow;
            userProfile.Address = payload.Address;

            await _unitOfWork.SaveAsync(cancellationToken);
                
            response.Data = new UpdateData
            {
                UserId = request.Id,
                FullName = userProfile.FullName,
                PhoneNumber = userProfile.PhoneNumber,
                Address = userProfile.Address,
                UpdatedAt = userProfile.UpdatedAt
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