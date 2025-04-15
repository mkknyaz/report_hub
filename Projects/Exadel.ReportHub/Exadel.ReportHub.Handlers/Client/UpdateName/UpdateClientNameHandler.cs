using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.UpdateName;

public record UpdateClientNameRequest(Guid Id, string Name) : IRequest<ErrorOr<Updated>>;
public class UpdateClientNameHandler(IClientRepository clientRepository) : IRequestHandler<UpdateClientNameRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateClientNameRequest request, CancellationToken cancellationToken)
    {
        var isExists = await clientRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await clientRepository.UpdateNameAsync(request.Id, request.Name, cancellationToken);

        return Result.Updated;
    }
}
