using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Roatp.Application.Mediatr.Responses;

namespace SFA.DAS.Roatp.Application.Mediatr.Behaviors
{
    [ExcludeFromCodeCoverage]
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : ValidatedResponseBase
        where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _compositeValidator;
        private readonly ILogger<TRequest> _logger;

        public ValidationBehavior(IValidator<TRequest> compositeValidator, ILogger<TRequest> logger)
        {
            _compositeValidator = compositeValidator;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogTrace("Validating Roatp API request");

            var result = await _compositeValidator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                var responseType = typeof(TResponse);

                if (responseType.IsGenericType)
                {
                    var resultType = responseType.GetGenericArguments()[0];
                    Type invalidResponseType;

                    if (responseType.Name.Contains("ValidatedResponse"))
                    {
                        invalidResponseType = typeof(ValidatedResponse<>).MakeGenericType(resultType);
                    }
                    else
                    {
                        invalidResponseType = typeof(ValidatedResponse<>).MakeGenericType(resultType);

                    }

                    if (Activator.CreateInstance(invalidResponseType,
                            result.Errors) as TResponse is { } invalidResponse)
                    {
                        return invalidResponse;
                    }
                }
            }

            _logger.LogTrace("Validation passed");

            var response = await next();

            return response;
        }
    }

    //
    // [ExcludeFromCodeCoverage]
    // public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    //     where TRequest : IRequest<TResponse>
    // {
    //     private readonly IEnumerable<IValidator<TRequest>> _validators;
    //
    //     public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    //     {
    //         _validators = validators;
    //     }
    //
    //     public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    //     {
    //         var context = new ValidationContext<TRequest>(request);
    //
    //         var failures = _validators
    //             .Select(x => x.ValidateAsync(context).GetAwaiter().GetResult())
    //             .SelectMany(x => x.Errors)
    //             .Where(x => x != null)
    //             .ToList();
    //
    //         if (failures.Any())
    //         {
    //             throw new ValidationException(failures);
    //         }
    //
    //         return next();
    //     }
    // }
}
