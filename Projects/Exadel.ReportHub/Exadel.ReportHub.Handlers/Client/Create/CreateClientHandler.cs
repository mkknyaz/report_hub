using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.Create;

public record CreateClientRequest(CreateClientDTO CreateClientDto) : IRequest<ErrorOr<ClientDTO>>;
public class CreateClientHandler(
    IClientRepository clientRepository,
    IMapper mapper,
    ICountryBasedEntityManager countryBasedEntityManager) : IRequestHandler<CreateClientRequest, ErrorOr<ClientDTO>>
{
    public async Task<ErrorOr<ClientDTO>> Handle(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var client = await countryBasedEntityManager.GenerateEntityAsync<CreateClientDTO, Data.Models.Client>(request.CreateClientDto, cancellationToken);

        await clientRepository.AddAsync(client, cancellationToken);

        var clientDto = mapper.Map<ClientDTO>(client);
        return clientDto;
    }
}
