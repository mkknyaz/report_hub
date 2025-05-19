using System.Security.Claims;
using Duende.IdentityModel;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Exadel.ReportHub.SDK.DTOs.User;
using Exadel.ReportHub.UI.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Exadel.ReportHub.Blazor.UI.Services;

public class AuthenticationService : IAuthService
{
	private readonly IUserStateService _userStateService;
	private readonly IHttpClientFactory _httpClientFactory;

	public AuthenticationService(IUserStateService userStateService, IHttpClientFactory httpClientFactory)
	{
		_userStateService = userStateService;
		_httpClientFactory = httpClientFactory;
	}

	public async Task<string> LoginAsync(string email, string password)
	{
		var client = _httpClientFactory.CreateClient("auth");
		var parameters = new Dictionary<string, string>
		{
			{ OidcConstants.TokenRequest.ClientId, Constants.Authentication.ClientId },
			{ OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password },
			{ OidcConstants.TokenRequest.UserName, email },
			{ OidcConstants.TokenRequest.Password, password },
			{ OidcConstants.TokenRequest.Scope, Constants.Authentication.Scope }
		};

		var response = await client.PostAsync("connect/token", new FormUrlEncodedContent(parameters));

		if (!response.IsSuccessStatusCode)
		{
			return null;
		}

		var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();
		var token = loginResult.AccessToken;

		_userStateService.SetUserToken(token);

		return token;
	}

    public Task LogoutAsync()
    {

        _userStateService.SetUserToken(null);
		_userStateService.SetUserClients([]);
        _userStateService.SetClient(Guid.Empty);

        return Task.CompletedTask;
    }
}