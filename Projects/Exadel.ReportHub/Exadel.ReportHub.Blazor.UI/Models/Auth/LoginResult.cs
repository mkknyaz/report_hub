using System.Text.Json.Serialization;
using Duende.IdentityModel;

namespace Exadel.ReportHub.UI.Models.Auth;

public class LoginResult
{
	[JsonPropertyName(OidcConstants.TokenResponse.AccessToken)]
	public string AccessToken { get; set; }
}