using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Item.Delete;

public record DeleteItemRequest(Guid Id) : IRequest<ErrorOr<Deleted>>;

public class DeleteItemHandler(IItemRepository itemRepository) : IRequestHandler<DeleteItemRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteItemRequest request, CancellationToken cancellationToken)
    {
        if (!await itemRepository.ExistsAsync(request.Id, cancellationToken))
        {
            return Error.NotFound();
        }

        await itemRepository.SoftDeleteAsync(request.Id, cancellationToken);
        return Result.Deleted;
    }
}
