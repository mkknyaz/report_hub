using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Exadel.ReportHub.Blazor;

public class AuthStateProvider(IJSRuntime js) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await js.InvokeAsync<string>(Constants.Storage.GetItem, OidcConstants.TokenResponse.AccessToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = ParseClaimsFromJwt(token);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task NotifyLoginAsync(string token)
    {
        var identity = ParseClaimsFromJwt(token);
        var claims = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claims)));

        await js.InvokeVoidAsync(Constants.Storage.SetItem, OidcConstants.TokenResponse.AccessToken, token);
    }

    public async Task NotifyLogoutAsync()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));

        await js.InvokeVoidAsync(Constants.Storage.RemoveItem, OidcConstants.TokenResponse.AccessToken);
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
