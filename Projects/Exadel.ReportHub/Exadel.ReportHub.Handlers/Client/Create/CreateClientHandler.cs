using ErrorOr;
using Exadel.ReportHub.Handlers.Managers.Client;
using Exadel.ReportHub.SDK.DTOs.Client;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.Create;

public record CreateClientRequest(CreateClientDTO CreateClientDto) : IRequest<ErrorOr<ClientDTO>>;
public class CreateClientHandler(IClientManager clientManager) : IRequestHandler<CreateClientRequest, ErrorOr<ClientDTO>>
{
    public async Task<ErrorOr<ClientDTO>> Handle(CreateClientRequest request, CancellationToken cancellationToken)
    {
        return await clientManager.CreateClientAsync(request.CreateClientDto, cancellationToken);
    }
}
