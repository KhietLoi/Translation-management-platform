using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Language.Queries.GetLanguages;

public class GetLanguagesHandler : IRequestHandler<GetLanguagesQuery, GetLanguagesResponse>
{
    private readonly ILogger<GetLanguagesHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetLanguagesHandler
    (
        ILogger<GetLanguagesHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetLanguagesQuery, GetLanguagesResponse>

    public async Task<GetLanguagesResponse> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetLanguagesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetLanguagesResponse();

        try
        {
            var languages = await _unitOfWork.Language
                .GetAll()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            if (!languages.Any() || languages.Count == 0)
            {
                _logger.LogInformation("{FunctionName} No languages found.", functionName);
                
                response.ErrorMessage = "No languages found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetLanguageResult
            {
                Languages = languages.Select(x => new GetLanguageData
                {
                    LanguageId = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).ToList()
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