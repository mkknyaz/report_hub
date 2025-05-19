using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Duende.IdentityModel;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Microsoft.AspNetCore.Components.Authorization;


namespace Exadel.ReportHub.Blazor.UI;

public class AuthStateProvider(IUserStateService userStateService) : AuthenticationStateProvider
{
	public override Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		var token = userStateService.GetUserToken();

		if (string.IsNullOrWhiteSpace(token))
		{
			return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
		}

		var identity = ParseClaimsFromJwt(token);
		return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
	}

	private ClaimsIdentity ParseClaimsFromJwt(string jwt)
	{
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(jwt);

		var claims = token.Claims.ToList();

		var roleClaims = token.Claims.Where(c => c.Type == JwtClaimTypes.Role).ToList();
		roleClaims.ForEach(role => claims.Add(new Claim(JwtClaimTypes.Role, role.Value)));

		return new ClaimsIdentity(claims, JwtClaimTypes.JwtTypes.AccessToken);
	}
}