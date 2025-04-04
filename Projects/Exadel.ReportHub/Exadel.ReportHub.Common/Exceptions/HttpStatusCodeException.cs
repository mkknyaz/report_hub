using System.Net;

namespace Exadel.ReportHub.Common.Exceptions;

public class HttpStatusCodeException : Exception
{
    public int StatusCode { get; }

    public IList<string> Errors { get; } = new List<string>();

    public HttpStatusCodeException(int statusCode)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCodeException(int statusCode, IList<string> errors)
        : base(string.Join(',', errors))
    {
        Errors = errors;
        StatusCode = statusCode;
    }
}
