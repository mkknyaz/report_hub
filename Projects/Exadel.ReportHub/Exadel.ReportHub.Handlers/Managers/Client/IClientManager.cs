using Exadel.ReportHub.SDK.DTOs.Client;

namespace Exadel.ReportHub.Handlers.Managers.Client;

public interface IClientManager
{
    Task<ClientDTO> CreateClientAsync(CreateClientDTO createClientDto, CancellationToken cancellationToken);

    Task<IList<ClientDTO>> CreateClientsAsync(IEnumerable<CreateClientDTO> createClientDtos, CancellationToken cancellationToken);
}
