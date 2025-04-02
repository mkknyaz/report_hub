using ErrorOr;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseService : ControllerBase
{
    protected IActionResult FromResult(ErrorOr<Created> result)
    {
        return result.Match(
            _ => Created(),
            errors => GetErrorResult(errors));
    }

    protected IActionResult FromResult(ErrorOr<Updated> result)
    {
        return result.Match(
            _ => NoContent(),
            errors => GetErrorResult(errors));
    }

    protected IActionResult FromResult(ErrorOr<Deleted> result)
    {
        return result.Match(
            _ => NoContent(),
            errors => GetErrorResult(errors));
    }

    protected IActionResult FromResult<TResult>(ErrorOr<TResult> result, int statusCode = StatusCodes.Status200OK)
        where TResult : class
    {
        return result.Match(
            value => StatusCode(statusCode, value),
            errors => GetErrorResult(errors));
    }

    private IActionResult GetErrorResult(List<Error> errors)
    {
        var errorResponse = new ErrorResponse
        {
            Errors = errors.Select(e => e.Description).ToList()
        };
        var statusCode = StatusCodes.Status500InternalServerError;

        if (errors.Any(e => e.Type == ErrorType.Validation))
        {
            statusCode = StatusCodes.Status400BadRequest;
        }
        else if(errors.Any(e => e.Type == ErrorType.Forbidden))
        {
            statusCode = StatusCodes.Status403Forbidden;
        }
        else if (errors.Any(e => e.Type == ErrorType.NotFound))
        {
            statusCode = StatusCodes.Status404NotFound;
        }

        return StatusCode(statusCode, errorResponse);
    }
}
