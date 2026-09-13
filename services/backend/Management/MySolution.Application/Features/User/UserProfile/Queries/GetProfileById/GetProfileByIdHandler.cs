using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.User.UserProfile.Queries.GetProfileById;

public class GetProfileByIdHandler : IRequestHandler<GetProfileByIdQuery, GetProfileByIdResponse>
{
    private readonly ILogger<GetProfileByIdHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetProfileByIdHandler
    (
        ILogger<GetProfileByIdHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetProfileByIdQuery, GetProfileByIdResponse>

    public async Task<GetProfileByIdResponse> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetProfileByIdHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetProfileByIdResponse();

        try
        {
            var userProfile = await _unitOfWork.UserProfile
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(up => up.UserId == request.Id, cancellationToken);
            
            if (userProfile == null)
            {
                response.ErrorMessage = "Profile not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetProfileByIdData
            {
                UserId = userProfile.UserId,
                FullName = userProfile.FullName,
                BirthDate = userProfile.BirthDate,
                PhoneNumber = userProfile.PhoneNumber,
                AvatarBlobName = userProfile.AvatarBlobName,
                Address = userProfile.Address
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