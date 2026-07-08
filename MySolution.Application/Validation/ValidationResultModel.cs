using System.Text.Json;
using FluentValidation.Results;

namespace MySolution.Application.Validation;

/// <summary>
/// Chứa toàn bộ kết quả validation
/// </summary>
public class ValidationResultModel
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = "Validation Failed";

    public List<ValidationError>? Errors { get; set; }

    public ValidationResultModel()
    {
        
    }

    public ValidationResultModel(ValidationResult result = null!)
    {
        Errors = result?.Errors
            .Select(error => new ValidationError(error.PropertyName,error.ErrorMessage, error.ErrorCode))
            .ToList();
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}