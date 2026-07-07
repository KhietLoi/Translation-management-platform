using System.Runtime.Serialization;

namespace MySolution.Application.Validation;

// Exception chuyên dành cho validation

[Serializable]
public class ValidationException : ExceptionError
{
    public ValidationResultModel ValidationResultModel { get; }

    public ValidationException(ValidationResultModel validationResultModel) : base(validationResultModel.ToString())
    {
        this.ValidationResultModel = validationResultModel;
    }

    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}