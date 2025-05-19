using System.Net.Http.Headers;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;

namespace Exadel.ReportHub.Blazor.UI.Handlers;

public class TokenAuthorizationHandler : DelegatingHandler
{
	private readonly IUserStateService _userStateService;

	public TokenAuthorizationHandler(IUserStateService userStateService)
	{
		_userStateService = userStateService;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var token = _userStateService.GetUserToken();
		if (!string.IsNullOrEmpty(token))
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		return await base.SendAsync(request, cancellationToken);
	}
}