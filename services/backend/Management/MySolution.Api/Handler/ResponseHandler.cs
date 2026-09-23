using Microsoft.AspNetCore.Mvc;
using MySolution.Application.Common.Models;

namespace MySolution.Api.Handler;

public static class ResponseHandler
{
    // ---------------------------
    // SUCCESS RESPONSES
    // ---------------------------

    public static ObjectResult Ok<T>(
        T? data = default,
        PagingInfo? paging = null)
    {
        return new ObjectResult(BaseResponse<T>.Ok(data, paging))
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public static ObjectResult Created<T>(
        T? data = default)
    {
        return new ObjectResult(BaseResponse<T>.Ok(data))
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    public static ObjectResult NoContent()
    {
        return new ObjectResult(BaseResponse.Ok())
        {
            StatusCode = StatusCodes.Status204NoContent
        };
    }


    // ---------------------------
    // ERROR RESPONSES
    // ---------------------------

    public static ObjectResult BadRequest(string message, string? code = null)
    {
        return new ObjectResult(BaseResponse.Fail(message, code))
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }

    public static ObjectResult Unauthorized(string message = "", string? code = null)
    {
        message = string.IsNullOrEmpty(message) ? "Unauthorized" : message;
        return new ObjectResult(BaseResponse.Fail(message, code))
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }

    public static ObjectResult Forbidden(string message, string? code = null)
    {
        return new ObjectResult(BaseResponse.Fail(message, code))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    public static ObjectResult NotFound(string message, string? code = null)
    {
        return new ObjectResult(BaseResponse.Fail(message, code))
        {
            StatusCode = StatusCodes.Status404NotFound
        };
    }

    public static ObjectResult Unsupported(string message, string? code = null)
    {
        return new ObjectResult(BaseResponse.Fail(message, code))
        {
            StatusCode = StatusCodes.Status415UnsupportedMediaType
        };
    }

    public static ObjectResult Error(string message, string? code = null)
    {
        return new ObjectResult(BaseResponse.Fail(message, code))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }


    // ---------------------------
    // LEGACY RESPONSE MAPPING
    // ---------------------------

    public static ObjectResult ToLegacyResponse<T>(
        int status,
        T? data = default,
        string message = "",
        string? messageCode = null,
        PagingInfo? paging = null)
    {
        return status switch
        {
            StatusCodes.Status200OK =>
                Ok(data, paging),

            StatusCodes.Status201Created =>
                Created(data),

            StatusCodes.Status204NoContent =>
                NoContent(),

            StatusCodes.Status400BadRequest =>
                BadRequest(message, messageCode),

            StatusCodes.Status401Unauthorized =>
                Unauthorized(message, messageCode),

            StatusCodes.Status403Forbidden =>
                Forbidden(message, messageCode),

            StatusCodes.Status404NotFound =>
                NotFound(message, messageCode),

            StatusCodes.Status415UnsupportedMediaType =>
                Unsupported(message, messageCode),

            _ =>
                Error(message, messageCode)
        };
    }
}