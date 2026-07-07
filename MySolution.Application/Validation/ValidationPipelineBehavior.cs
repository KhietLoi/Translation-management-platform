using FluentValidation;
using MediatR;

namespace MySolution.Application.Validation;

/// <summary>
/// Tự động chạy FluentValidation trước khi Handler được thực thi
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    //Inject tất cả các validator của request vào đây 
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this._validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationErrors = _validators
            .Select(validator => validator.Validate(request))
            .SelectMany(result => result.Errors)
            .Select(x => new ValidationError
            {
                Field = x.PropertyName,
                ErrorMessage = x.ErrorMessage,
                ErrorMessageCode = x.ErrorCode
            })
            .ToList();

        if (validationErrors.Any())
        {
            var validationResultModel = new ValidationResultModel
            {
                Errors = validationErrors
            };
            var validationException = new ValidationException(validationResultModel);
            throw validationException;
        }

        return await next();
    }
}