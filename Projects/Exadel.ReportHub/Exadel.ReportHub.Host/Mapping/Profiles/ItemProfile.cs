using AutoMapper;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.SDK.DTOs.Item;

namespace Exadel.ReportHub.Host.Mapping.Profiles;

public class ItemProfile : Profile
{
    public ItemProfile()
    {
        CreateMap<Item, ItemDTO>();
        CreateMap<CreateUpdateItemDTO, Item>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore())
            .ForMember(x => x.CurrencyCode, opt => opt.Ignore());
    }
}
