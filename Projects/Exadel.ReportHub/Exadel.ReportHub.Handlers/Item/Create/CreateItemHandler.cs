using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;

namespace Exadel.ReportHub.Handlers.Item.Create;

public record CreateItemRequest(CreateUpdateItemDTO CreateItemDto) : IRequest<ErrorOr<ItemDTO>>;
public class CreateItemHandler(IItemRepository itemRepository, ICurrencyRepository currencyRepository, IMapper mapper) : IRequestHandler<CreateItemRequest, ErrorOr<ItemDTO>>
{
    public async Task<ErrorOr<ItemDTO>> Handle(CreateItemRequest request, CancellationToken cancellationToken)
    {
        var item = mapper.Map<Data.Models.Item>(request.CreateItemDto);
        item.Id = Guid.NewGuid();
        item.CurrencyCode = await currencyRepository.GetCodeByIdAsync(request.CreateItemDto.CurrencyId, cancellationToken);

        await itemRepository.AddAsync(item, cancellationToken);

        return mapper.Map<ItemDTO>(item);
    }
}
