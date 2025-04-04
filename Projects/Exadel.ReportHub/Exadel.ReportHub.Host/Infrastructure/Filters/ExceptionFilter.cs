using Exadel.ReportHub.Common.Exceptions;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Exadel.ReportHub.Host.Infrastructure.Filters;

public class ExceptionFilter(ILogger<ExceptionFilter> logger, IHostEnvironment hostEnvironment) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An exception occurred while processing the request");

        if (context.Exception is HttpStatusCodeException httpStatusCodeException)
        {
            context.Result = CreateStatusCodeErrorResult(httpStatusCodeException);
        }
        else
        {
            context.Result = CreateErrorResult(context.Exception, hostEnvironment);
        }

        context.ExceptionHandled = true;
    }

    private IActionResult CreateStatusCodeErrorResult(HttpStatusCodeException exception)
    {
        return new ObjectResult(new ErrorResponse { Errors = exception.Errors })
        {
            StatusCode = exception.StatusCode
        };
    }

    private IActionResult CreateErrorResult(Exception exception, IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment.IsDevelopment())
        {
            return new ObjectResult(new ErrorResponse { Errors = new List<string> { exception.Message } });
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}
