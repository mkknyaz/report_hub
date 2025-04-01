using System.Net;
using Exadel.ReportHub.Host.Infrastructure.Exceptions;
using FluentValidation;
using MediatR;

namespace Exadel.ReportHub.Host.Mediatr;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationResults = await Task.WhenAll(_validators.
            Select(v => v.ValidateAsync(request, cancellationToken)));

        var failures = validationResults
            .SelectMany(validationResult => validationResult.Errors)
            .Where(failures => failures != null)
            .Select(validationFailure => validationFailure.ErrorMessage)
            .Distinct()
            .ToList();

        if (failures.Any())
        {
            throw new HttpStatusCodeException(failures, HttpStatusCode.BadRequest);
        }

        return await next();
    }
}
