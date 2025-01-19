using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace SharedKernel;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var resultErrors = validationResults.SelectMany(r => r.AsErrors()).Where(f => f != null).Distinct().ToArray();
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (resultErrors is not null && resultErrors.Length != 0)
            {
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var result = typeof(Result<>)
                        .GetGenericTypeDefinition()
                        .MakeGenericType(typeof(Result).GetGenericArguments()[0])
                        .GetMethod(nameof(Result.Invalid))
                        .Invoke(null, resultErrors);

                    if (result is not null)
                    {
                        return (TResponse)result;

                    }
                }

                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
