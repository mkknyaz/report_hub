using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;

namespace Exadel.ReportHub.Handlers.Item.Update;

public record UpdateItemRequest(Guid Id, CreateUpdateItemDTO UpdateItemDto) : IRequest<ErrorOr<Updated>>;

public class UpdateItemHandler(IItemRepository itemRepository, ICurrencyRepository currencyRepository, IMapper mapper) : IRequestHandler<UpdateItemRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateItemRequest request, CancellationToken cancellationToken)
    {
        var itemClientId = await itemRepository.GetClientIdAsync(request.Id, cancellationToken);
        if (itemClientId == null)
        {
            return Error.NotFound();
        }

        if (itemClientId != request.UpdateItemDto.ClientId)
        {
            return Error.Validation(description: Constants.Validation.Item.ClientIdImmutable);
        }

        var item = mapper.Map<Data.Models.Item>(request.UpdateItemDto);
        item.Id = request.Id;
        item.CurrencyCode = await currencyRepository.GetCodeByIdAsync(request.UpdateItemDto.CurrencyId, cancellationToken);

        await itemRepository.UpdateAsync(item, cancellationToken);
        return Result.Updated;
    }
}
