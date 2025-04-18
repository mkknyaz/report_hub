using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;

namespace Exadel.ReportHub.Handlers.Item.GetById;

public record GetItemByIdRequest(Guid Id) : IRequest<ErrorOr<ItemDTO>>;

public class GetItemByIdHandler(IItemRepository itemRepository, IMapper mapper) : IRequestHandler<GetItemByIdRequest, ErrorOr<ItemDTO>>
{
    public async Task<ErrorOr<ItemDTO>> Handle(GetItemByIdRequest request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
        {
            return Error.NotFound();
        }

        return mapper.Map<ItemDTO>(item);
    }
}
