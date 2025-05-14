using AutoMapper;
using Exadel.ReportHub.Handlers.Managers.Helpers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;

namespace Exadel.ReportHub.Handlers.Managers.Client;

public class ClientManager(IClientRepository clientRepository, ICountryDataFiller countryBasedHelper, IMapper mapper) : IClientManager
{
    public async Task<ClientDTO> CreateClientAsync(CreateClientDTO createClientDto, CancellationToken cancellationToken)
    {
        return (await CreateClientsAsync([createClientDto], cancellationToken)).Single();
    }

    public async Task<IList<ClientDTO>> CreateClientsAsync(IEnumerable<CreateClientDTO> createClientDtos, CancellationToken cancellationToken)
    {
        var clients = mapper.Map<IList<Data.Models.Client>>(createClientDtos);

        await countryBasedHelper.FillCountryDataAsync(clients, cancellationToken);

        foreach (var client in clients)
        {
            client.Id = Guid.NewGuid();
        }

        await clientRepository.AddManyAsync(clients, cancellationToken);

        return mapper.Map<IList<ClientDTO>>(clients);
    }
}
