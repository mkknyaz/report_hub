using Exadel.ReportHub.SDK.DTOs.User;

namespace Exadel.ReportHub.Blazor.UI.Services.Abstractions;

public interface IClientsService
{
	Task<IList<UserClientDTO>> GetClientsAsync();
}