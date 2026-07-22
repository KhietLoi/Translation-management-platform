using System.Net;
using Microsoft.AspNetCore.Mvc;
using MySolution.Application.Common.Models;

namespace MySolution.Api.Helpers;

public static class ResponseHelper
{
    public static ObjectResult ToResponse(HttpStatusCode httpStatusCode, BaseResponse baseResponse, object? data = null)
    {
        return new ObjectResult(new
            {
                success = baseResponse.Success,
                errorMessage = baseResponse.ErrorMessage,
                errorMessageCode = baseResponse.ErrorMessageCode,
                data
            })
            { StatusCode = (int)httpStatusCode };
    }
    
    public static ObjectResult ToPaginationResponse(int httpStatusCode, BaseResponse baseResponse, object? data = null)
    {
        return new ObjectResult(new
            {
                success = baseResponse.Success,
                errorMessage = baseResponse.ErrorMessage,
                errorMessageCode = baseResponse.ErrorMessageCode,
                data,
                paging = baseResponse.Paging
            })
            { StatusCode = httpStatusCode };
    }
}