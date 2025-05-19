namespace Exadel.ReportHub.Blazor.UI.Services.Abstractions;

public interface IAuthService
{
	Task<string> LoginAsync(string email, string password);

	Task LogoutAsync();
}