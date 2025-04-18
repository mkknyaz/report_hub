using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;

namespace Exadel.ReportHub.Handlers.Item.GetByClientId;

public record GetItemsByClientIdRequest(Guid ClientId) : IRequest<ErrorOr<IList<ItemDTO>>>;

public class GetItemsByClientIdHandler(IItemRepository itemRepository, IMapper mapper) : IRequestHandler<GetItemsByClientIdRequest, ErrorOr<IList<ItemDTO>>>
{
    public async Task<ErrorOr<IList<ItemDTO>>> Handle(GetItemsByClientIdRequest request, CancellationToken cancellationToken)
    {
        return mapper.Map<List<ItemDTO>>(await itemRepository.GetByClientIdAsync(request.ClientId, cancellationToken));
    }
}
