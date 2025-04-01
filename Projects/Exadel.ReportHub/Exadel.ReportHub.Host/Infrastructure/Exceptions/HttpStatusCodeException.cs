using System.Net;

namespace Exadel.ReportHub.Host.Infrastructure.Exceptions;

public class HttpStatusCodeException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public IList<string> Errors { get; }

    public HttpStatusCodeException(IList<string> errors, HttpStatusCode statusCode)
        : base(string.Join(',', errors))
    {
        Errors = errors;
        StatusCode = statusCode;
    }
}
