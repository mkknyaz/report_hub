using Exadel.ReportHub.SDK.DTOs.User;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;

namespace Exadel.ReportHub.Blazor.UI.Services;

public class UserStateService : IUserStateService
{
    private readonly UserState _userState = new();

    public string GetUserToken() => _userState.Token;

    public IList<UserClientDTO> GetUserClientDTOs() => _userState.UserClientDTOs;

    public Guid GetSelectedClientId() => _userState.SelectedClient;

    public UserClientDTO GetSelectedClient() => _userState.UserClientDTOs.FirstOrDefault(c => c.ClientId == _userState.SelectedClient);

    public UserRole GetCurrentRole() => _userState.UserClientDTOs.FirstOrDefault(c => c.ClientId == _userState.SelectedClient).Role;

    public void SetUserToken(string token) => _userState.Token = token;

    public void SetUserClients(IList<UserClientDTO> clientDtos) => _userState.UserClientDTOs = clientDtos;

    public void SetClient(Guid clientId) => _userState.SelectedClient = clientId;

}

internal class UserState
{
    public string Token { get; set; } = string.Empty;

    public IList<UserClientDTO> UserClientDTOs { get; set; } = new List<UserClientDTO>();

    public Guid SelectedClient { get; set; } = Guid.Empty;
}