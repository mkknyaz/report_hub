using System.Net;
using Exadel.ReportHub.Common.Exceptions;
using Exadel.ReportHub.Common.Providers;
using Microsoft.AspNetCore.Http;

namespace Exadel.ReportHub.RA;
public class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public Guid GetUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userClaim = user?.FindFirst("sub")?.Value;


        if (string.IsNullOrEmpty(userClaim))
        {
            throw new HttpStatusCodeException(StatusCodes.Status401Unauthorized, new List<string> {"User ID not found"});
        }

        return Guid.Parse(userClaim);
    }
}
