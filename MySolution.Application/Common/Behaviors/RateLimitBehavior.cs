using MediatR;
using MySolution.Application.Common.Exceptions;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.RateLimit;

namespace MySolution.Application.Common.Behaviors;

public sealed class RateLimitBehavior <TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRateLimitedRequest
{
    private readonly IRateLimitService _rateLimitService;
    
    public RateLimitBehavior(IRateLimitService rateLimitService)
    {
        _rateLimitService = rateLimitService;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var policy = request.GetRateLimitPolicy();
        var result = await _rateLimitService.CheckAsync(policy, cancellationToken);
        
        if (!result.Allowed)
        {
            throw new RateLimitExceededException("Rate limit exceeded.");
        }

        return await next();
    }
}