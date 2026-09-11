namespace MySolution.Application.Validation;

/// <summary>
///     Đại diện cho một lỗi Validation
/// </summary>
public class ValidationError
{
    public ValidationError()
    {
    }

    public ValidationError(string field, string errorMessage, string errorMessageCode)
    {
        Field = field != string.Empty ? field : null;
        ErrorMessage = errorMessage;
        ErrorMessageCode = errorMessageCode;
    }

    public string? Field { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorMessageCode { get; set; }
}