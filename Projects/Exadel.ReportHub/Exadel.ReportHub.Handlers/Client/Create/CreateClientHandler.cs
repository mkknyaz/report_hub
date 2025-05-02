using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.Create;

public record CreateClientRequest(CreateClientDTO CreateClientDto) : IRequest<ErrorOr<ClientDTO>>;
public class CreateClientHandler(
    IClientRepository clientRepository,
    IMapper mapper,
    ICountryRepository countryRepository) : IRequestHandler<CreateClientRequest, ErrorOr<ClientDTO>>
{
    public async Task<ErrorOr<ClientDTO>> Handle(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var country = await countryRepository.GetByIdAsync(request.CreateClientDto.CountryId, cancellationToken);

        var client = mapper.Map<Data.Models.Client>(request.CreateClientDto);
        client.Id = Guid.NewGuid();
        client.Country = country.Name;
        client.CurrencyId = country.CurrencyId;
        client.CurrencyCode = country.CurrencyCode;

        await clientRepository.AddAsync(client, cancellationToken);

        var clientDto = mapper.Map<ClientDTO>(client);
        return clientDto;
    }
}
