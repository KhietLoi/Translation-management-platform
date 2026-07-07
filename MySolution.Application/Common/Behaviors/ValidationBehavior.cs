using FluentValidation;
using MediatR;

namespace MySolution.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    //MediatR sẽ lấy tất cả Validator của request hiện tại
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) // Ktra có validator không
            return await next(); //--> Đi thẳng tới Handler
        
        //FluentValidation cần ValidationContext để chạy rule
        var context = new ValidationContext<TRequest>(request);
        
        //Chạy toàn bộ Validators
        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        //Gom toàn bộ lỗi
        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        
        //Ném lỗi
        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}