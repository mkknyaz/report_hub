using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.Delete;

public record DeleteClientRequest(Guid Id) : IRequest<ErrorOr<Deleted>>;

public class DeleteClientHandler(IClientRepository clientRepository) : IRequestHandler<DeleteClientRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteClientRequest request, CancellationToken cancellationToken)
    {
        var isExists = await clientRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await clientRepository.SoftDeleteAsync(request.Id, cancellationToken);

        return Result.Deleted;
    }
}
