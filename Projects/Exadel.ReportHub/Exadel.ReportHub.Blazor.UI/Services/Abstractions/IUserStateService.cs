using Exadel.ReportHub.SDK.DTOs.User;
using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Blazor.UI.Services.Abstractions;

public interface IUserStateService
{
	public string GetUserToken();

    public IList<UserClientDTO> GetUserClientDTOs();

    public Guid GetSelectedClientId();

    public UserClientDTO? GetSelectedClient();

    public UserRole GetCurrentRole();

    public void SetUserToken(string token);

    public void SetUserClients(IList<UserClientDTO> clientDtos);

    public void SetClient(Guid clientId);
}