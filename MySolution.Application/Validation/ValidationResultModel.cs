using System.Net;
using System.Text.Json;
using FluentValidation.Results;

namespace MySolution.Application.Validation;

/// <summary>
///     Chứa toàn bộ kết quả validation
/// </summary>
public class ValidationResultModel
{
    public ValidationResultModel()
    {
    }

    public ValidationResultModel(ValidationResult result = null!)
    {
        Errors = result?.Errors
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage, error.ErrorCode))
            .ToList();
    }

    public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
    public string Message { get; set; } = "Validation Failed";

    public List<ValidationError>? Errors { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}