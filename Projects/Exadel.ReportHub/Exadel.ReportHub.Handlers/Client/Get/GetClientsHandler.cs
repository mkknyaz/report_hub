using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.Get;

public record GetClientsRequest : IRequest<ErrorOr<IList<ClientDTO>>>;

public class GetClientsHandler(IClientRepository clientRepository, IMapper mapper) : IRequestHandler<GetClientsRequest, ErrorOr<IList<ClientDTO>>>
{
    public async Task<ErrorOr<IList<ClientDTO>>> Handle(GetClientsRequest request, CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetAsync(cancellationToken);

        var clientDtos = mapper.Map<List<ClientDTO>>(clients);
        return clientDtos;
    }
}
