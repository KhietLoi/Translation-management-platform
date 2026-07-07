using System.Net;
using MySolution.Application.Common.Extensions;

namespace MySolution.Application.Common.Model;

public class PagingInfo
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public long TotalItem { get; set; }
    public int TotalPage { get; set; }
}

public class BaseResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorMessageCode { get; set; }
    public PagingInfo? Paging { get; set; }

    public static BaseResponse Ok() => new() { Success = true };

    public static BaseResponse Fail(string message, string? code = null)
        => new() { Success = false, ErrorMessage = message, ErrorMessageCode = code };

    public BaseResponse WithMessage(Enum status)
    {
        ErrorMessage = status.Localize();
        ErrorMessageCode = status.Code();
        return this;
    }
    
    public BaseResponse WithSuccess(bool success)
    {
        Success = success;
        return this;
    }

    public BaseResponse WithStatus(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        return this;
    }
}

public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }

    public BaseResponse<T> WithData(T data)
    {
        Data = data;
        return this;
    }
    
    public static BaseResponse<T> Ok(T data, PagingInfo? paging = null)
        => new()
        {
            Success = true,
            Data = data,
            Paging = paging
        };

    public new static BaseResponse<T> Fail(string message, string? code = null)
        => new()
        {
            Success = false,
            ErrorMessage = message,
            ErrorMessageCode = code
        };
}