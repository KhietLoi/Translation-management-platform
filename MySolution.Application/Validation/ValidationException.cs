using System.Runtime.Serialization;

namespace MySolution.Application.Validation;

// Exception chuyên dành cho validation
[Serializable]
public class ValidationException : ExceptionError
{
    public ValidationException(ValidationResultModel validationResultModel) : base(validationResultModel.ToString())
    {
        ValidationResultModel = validationResultModel;
    }

    [Obsolete("Obsolete")]
    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ValidationResultModel ValidationResultModel { get; } = null!;
}