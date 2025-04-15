using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Handlers.User.Create;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.Create;

public record CreateClientRequest(CreateClientDTO CreateClientDto) : IRequest<ErrorOr<ClientDTO>>;
public class CreateClientHandler(IClientRepository clientRepository, IMapper mapper) : IRequestHandler<CreateClientRequest, ErrorOr<ClientDTO>>
{
    public async Task<ErrorOr<ClientDTO>> Handle(CreateClientRequest request, CancellationToken cancellationToken)
    {
        var client = mapper.Map<Data.Models.Client>(request.CreateClientDto);
        client.Id = Guid.NewGuid();

        await clientRepository.AddAsync(client, cancellationToken);

        var clientDto = mapper.Map<ClientDTO>(client);
        return clientDto;
    }
}
