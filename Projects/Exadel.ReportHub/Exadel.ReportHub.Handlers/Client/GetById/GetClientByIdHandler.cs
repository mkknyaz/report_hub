using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.GetById;

public record GetClientByIdRequest(Guid Id) : IRequest<ErrorOr<ClientDTO>>;

public class GetClientByIdHandler(IClientRepository clientRepository, IMapper mapper) : IRequestHandler<GetClientByIdRequest, ErrorOr<ClientDTO>>
{
    public async Task<ErrorOr<ClientDTO>> Handle(GetClientByIdRequest request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (client is null)
        {
            return Error.NotFound();
        }

        var clientDto = mapper.Map<ClientDTO>(client);
        return clientDto;
    }
}
