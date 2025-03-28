using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseService : ControllerBase
{
    protected IActionResult FromResult(ErrorOr<Created> result)
    {
        return result.Match(
            success => Created(),
            errors => GetErrorResult(errors));
    }

    protected IActionResult FromResult(ErrorOr<Updated> result)
    {
        return result.Match(
            success => NoContent(),
            errors => GetErrorResult(errors));
    }

    protected IActionResult FromResult(ErrorOr<Deleted> result)
    {
        return result.Match(
            success => NoContent(),
            errors => GetErrorResult(errors));
    }

    protected IActionResult FromResult<TResult>(ErrorOr<TResult> result)
        where TResult : class
    {
        return result.Match(
            success => Ok(success),
            errors => GetErrorResult(errors));
    }

    private IActionResult GetErrorResult(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Problem();
        }

        return GetErrorResult(errors[0]);
    }

    private IActionResult GetErrorResult(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: error.Description);
    }
}
